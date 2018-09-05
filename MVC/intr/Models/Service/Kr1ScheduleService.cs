
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq.Dynamic;
using System.Linq;
using IntranetSystem.Models;
using IntranetSystem.Kr1ServiceReference;
using System.ServiceModel;
using System.Net;
using System.Text;
using IntranetSystem.Poco;

using System.Data.SqlClient;
using System.Transactions;
using System.Configuration;
using System;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using System.Linq.Dynamic;
using Quartz;



namespace IntranetSystem.Service
{
    public class Kr1ScheduleService:IJob
{

    private Entities db = new Entities();
    private GatewayAPISoapClient kr1Client;
    private CommonService commonService = new CommonService();
    private const String url = "http://122.147.252.5:88/API/GatewayAPI.asmx";
  
 




   public void initVLan_Schedule(){
       //doCreateVLanDB();
   }


 


    public int doRunSchedule()
    {
        int i = 0;
        try
        {
            var endPoint = new EndpointAddress(url);
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.MaxReceivedMessageSize = int.MaxValue;
            GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
            kr1Client.ClientCredentials.UserName.UserName = "root";
            kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
            Array VLanDefineClass = kr1Client.ListVLanDefine();
            
            
            using (db)
            {                
                //db.ExecuteStoreCommand("DELETE FROM VLAN_SCHEDULE");

                foreach (VLanDefineClass item in VLanDefineClass)
                {
                    Boolean isNew = false;          
                    VLAN_SCHEDULE obj = (from t in db.VLAN_SCHEDULE where t.VLAN == item.VLanID select t).SingleOrDefault();
                    if (obj == null)
                    {
                        isNew = true;
                        obj = new VLAN_SCHEDULE();                                                
                        obj.SDT = null;
                        obj.EDT = null;
                    }
                    obj.VLAN = item.VLanID;
                    obj.DHCPIP = item.DHCPStartIP;
                    obj.UP = item.BandwidthUp;
                    obj.DOWN = item.BandwidthDown;
                    obj.SESSION_CNT = item.SessionCountMax;

                    obj.DF_DHCPIP = item.DHCPStartIP;
                    obj.DF_UP = item.BandwidthUp;
                    obj.DF_DOWN = item.BandwidthDown;
                    obj.DF_SESSION_CNT = item.SessionCountMax;

                    if (isNew)
                    {
                        db.VLAN_SCHEDULE.AddObject(obj);
                    }
                }
                i = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
        }
        finally
        {
            // kr1Client.Close();
        }

        return i;
    }


    private void Log(string msg)
    {
        System.IO.File.AppendAllText(@"C:\Temp\VlanSchedulelog.txt", msg + Environment.NewLine);
    }



    public void Execute(IJobExecutionContext context)
    {
        int i = 0;
        String dhcp = "";
        try
        {
            using (db)
            {
            DateTime nowdt = System.DateTime.Now;
            var endPoint = new EndpointAddress(url);
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.MaxReceivedMessageSize = int.MaxValue;
            GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
            kr1Client.ClientCredentials.UserName.UserName = "root";
            kr1Client.ClientCredentials.UserName.Password = "spadmin2075";            
                
                var query = (from t in db.VLAN_SCHEDULE where (nowdt >= t.SDT && nowdt <=t.EDT) && t.SCHED!="Y" select t);
                if (query != null)
                {
                    foreach(VLAN_SCHEDULE obj in query){                   

                        kr1Client.SetVLanDefineBandwidthUp( Convert.ToInt32(obj.VLAN), Convert.ToInt32(obj.UP));
                        kr1Client.SetVLanDefineBandwidthDown(Convert.ToInt32(obj.VLAN), Convert.ToInt32(obj.DOWN));
                        if (obj.ACTION == "Charge")
                        {
                            kr1Client.SetVLanDefineAction(Convert.ToInt32(obj.VLAN), enumVLanActionType.Charge);
                        }
                        else
                        {
                            kr1Client.SetVLanDefineAction(Convert.ToInt32(obj.VLAN), enumVLanActionType.Free);
                        }

                        if (StringUtils.getString(obj.DHCPIP) == "1")
                        {
                            dhcp = getPrivateDhcp();
                        }
                        else
                        {
                            dhcp = getPublicDhcp();
                        }
                        obj.SCHED = "Y";
                        kr1Client.SetVLanDefineDHCPIP(Convert.ToInt32(obj.VLAN), dhcp);

                        string msg = String.Format("{0:yyyy/MM/dd HH:mm:ss} [Change]VLAN:" + obj.VLAN + " DHCPIP:"+obj.DHCPIP +" DOWN:"+obj.DOWN + " UP:"+obj.UP, DateTime.Now);
                        Log(msg);

                       obj.DHCPIP = dhcp;
                      // obj.DOWN = obj.DOWN;
                      // obj.UP =   obj.UP;
                       obj.UPD_DT = System.DateTime.Now;
                      
                                                
                   }
                }

                //過期還原回default
                var query1 = (from t in db.VLAN_SCHEDULE where (nowdt > t.EDT) && t.SCHED == "Y" select t);
                if (query1 != null)
                {
                    foreach (VLAN_SCHEDULE obj in query1)
                    {

                        kr1Client.SetVLanDefineBandwidthUp(Convert.ToInt32(obj.VLAN), Convert.ToInt32(obj.DF_UP));
                        kr1Client.SetVLanDefineBandwidthDown(Convert.ToInt32(obj.VLAN), Convert.ToInt32(obj.DF_DOWN));

                        

                        obj.SCHED = "N";
                        kr1Client.SetVLanDefineDHCPIP(Convert.ToInt32(obj.VLAN), obj.DF_DHCPIP);
                        kr1Client.SetVLanDefineAction(Convert.ToInt32(obj.VLAN), enumVLanActionType.Charge);
                        obj.DHCPIP = obj.DF_DHCPIP;
                        obj.DOWN = obj.DF_DOWN;
                        obj.UP = obj.DF_UP;
                        obj.ACTION = "Charge";
                        obj.SDT = null;
                        obj.EDT = null;
                        string msg = String.Format("{0:yyyy/MM/dd HH:mm:ss} [TimeOut]VLAN:" + obj.VLAN + " DHCPIP:" + obj.DHCPIP + " DOWN:" + obj.DOWN + " UP:" + obj.UP, DateTime.Now);
                        Log(msg);
                        
                        obj.UPD_DT = System.DateTime.Now;

                    }
                }

                //更改RC房
                var imc = (from t in db.IMC_CODE where t.KIND == "VLAN" && t.TYPE == "RC" select t).SingleOrDefault();
                if (imc != null)
                {
//                    var query2 = (from t in db.VLAN_SCHEDULE where t.RC == "Y" && (t.DOWN != Convert.ToInt32(imc.CITEM1) || t.UP !=Convert.ToInt32(imc.CITEM2)) select t);
                    var query2 = (from t in db.VLAN_SCHEDULE select t);
                    if (query2 != null)
                    {
                        foreach (VLAN_SCHEDULE obj in query2)                              
                        {
                            if (obj.RC == "Y" && (obj.DOWN != Convert.ToInt32(imc.CITEM1) || obj.UP != Convert.ToInt32(imc.CITEM2)))
                            {

                                kr1Client.SetVLanDefineBandwidthDown(Convert.ToInt32(obj.VLAN), Convert.ToInt32(imc.CITEM1));
                                kr1Client.SetVLanDefineBandwidthUp(Convert.ToInt32(obj.VLAN), Convert.ToInt32(imc.CITEM2));
                                //kr1Client.SetVLanDefineDHCPIP(Convert.ToInt32(obj.VLAN), obj.DF_DHCPIP);                                
                                obj.DOWN = Convert.ToInt32(imc.CITEM1);
                                obj.UP = Convert.ToInt32(imc.CITEM2);
                                obj.DF_DOWN = Convert.ToInt32(imc.CITEM1);
                                obj.DF_UP = Convert.ToInt32(imc.CITEM2);
                                
                                string msg = String.Format("{0:yyyy/MM/dd HH:mm:ss} [RC房調整]VLAN:" + obj.VLAN + " DOWN:" + obj.DOWN + " UP:" + obj.UP, DateTime.Now);
                                Log(msg);
                            }
                            else if (obj.RC == "N" && (obj.DF_DOWN == Convert.ToInt32(imc.CITEM1) || obj.DF_UP == Convert.ToInt32(imc.CITEM2)) && StringUtils.getString(obj.SCHED) !="Y")
                            {
                                kr1Client.SetVLanDefineBandwidthDown(Convert.ToInt32(obj.VLAN), 3072);
                                kr1Client.SetVLanDefineBandwidthUp(Convert.ToInt32(obj.VLAN), 3072);
                                obj.DOWN = 3072;
                                obj.UP = 3072;
                                obj.DF_DOWN = 3072;
                                obj.DF_UP = 3072;
                                string msg = String.Format("{0:yyyy/MM/dd HH:mm:ss} [RC房調回預設]VLAN:" + obj.VLAN + " DOWN:3072 UP:3072", DateTime.Now);
                                Log(msg);

                            }
                        }
                    }
                }

                db.SaveChanges();

            }
        }
        catch (Exception ex)
        {
            
        }
        finally
        {
        }

    }

    public int doReSet(int VLAN){
        int i = 0;

        try
        {
            using (db)
            {

                var query = from t in db.VLAN_SCHEDULE select t;

                if (StringUtils.getString(VLAN) != "")
                {
                    query = query.Where(t => t.VLAN == VLAN);
                }

                foreach (VLAN_SCHEDULE item in query)
                {
                    item.DHCPIP = item.DF_DHCPIP;
                    item.DOWN = item.DF_DOWN;
                    item.UP = item.DF_UP;
                    item.SDT = null;
                    item.EDT = null;
                    item.SCHED = "N";
                }
              i=  db.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
        }
        return i;
    }


    public JObject getListVLanDefineIP(String vlanid,String mac)
    {
        JObject jo = new JObject();
        var endPoint = new EndpointAddress(url);
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        binding.MaxReceivedMessageSize = int.MaxValue;
        GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
        kr1Client.ClientCredentials.UserName.UserName = "root";
        kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
        VLanDefineClass[] vlan = kr1Client.ListVLanDefine();
        SubscribeClass item= GetSubscribeByMACAddress(mac);
        
        int sec = 0;
        if (item != null)
        {
            sec = item.CountdownSec;
        }
        jo.Add("SEC", sec);
        jo.Add("DHCP", vlan[0].DHCPStartIP);
        return jo;
    }

    public String getPrivateDhcp()
    {
        JArray ja = new JArray();
        var endPoint = new EndpointAddress(url);
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        binding.MaxReceivedMessageSize = int.MaxValue;
        GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
        kr1Client.ClientCredentials.UserName.UserName = "root";
        kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
        Array DhcpItemPrivate = kr1Client.GetDHCPPool(enumAddressPoolType.PrivateAddressPool);

        List<DHCPItemClass> dhcplist = new List<DHCPItemClass>();

        String dhcp = "";
        foreach (DHCPItemClass item in DhcpItemPrivate)
        {
            dhcp = item.StartIP;
        }

        return dhcp;


    }


    public String getPublicDhcp()
    {
        JArray ja = new JArray();
        var endPoint = new EndpointAddress(url);
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        binding.MaxReceivedMessageSize = int.MaxValue;
        GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
        kr1Client.ClientCredentials.UserName.UserName = "root";
        kr1Client.ClientCredentials.UserName.Password = "spadmin2075";        
        Array DhcpItemPublic = kr1Client.GetDHCPPool(enumAddressPoolType.PublicAddressPool);

        String dhcp = "";
        foreach (DHCPItemClass item in DhcpItemPublic)
        {        
            dhcp= item.StartIP;
        }

        return dhcp;


    }


    public JArray getDhcpPool()
    {
        JArray ja = new JArray();
        var endPoint = new EndpointAddress(url);
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        binding.MaxReceivedMessageSize = int.MaxValue;
        GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
        kr1Client.ClientCredentials.UserName.UserName = "root";
        kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
        Array DhcpItemPrivate = kr1Client.GetDHCPPool(enumAddressPoolType.PrivateAddressPool);
        Array DhcpItemPublic = kr1Client.GetDHCPPool(enumAddressPoolType.PublicAddressPool);
        List<DHCPItemClass> dhcplist = new List<DHCPItemClass>();
        foreach (DHCPItemClass item in DhcpItemPrivate)
        {           
            item.Netmask = "(Private)";
            dhcplist.Add(item);
        }
        foreach (DHCPItemClass item in DhcpItemPublic)
        {
            item.Netmask = "(Public)";
            dhcplist.Add(item);
        }
        var query = (from t in dhcplist select t).AsQueryable();
        foreach (var item in query)
        {

            var itemObject = new JObject
                    {   
                        {"ITEM",item.StartIP + item.Netmask},
                        {"VALUE",item.StartIP }
                        
                    };
            ja.Add(itemObject);
        }                
        return ja;


    }


    public void SessionDelete(String ip)
    {
        try
        {
            var endPoint = new EndpointAddress(url);
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.MaxReceivedMessageSize = int.MaxValue;
            GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
            kr1Client.ClientCredentials.UserName.UserName = "root";
            kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
            kr1Client.SessionDelete(ip);
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }  
    }


   public void doSave(int vid,int up,int down)
   {

       
       try
       {
           var endPoint = new EndpointAddress(url);
           var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
           binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
           binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
           binding.MaxReceivedMessageSize = int.MaxValue;
           GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
           kr1Client.ClientCredentials.UserName.UserName = "root";
           kr1Client.ClientCredentials.UserName.Password = "spadmin2075";           
           kr1Client.SetVLanDefineBandwidthUp(vid, up);
           kr1Client.SetVLanDefineBandwidthDown(vid, down);
       }catch(Exception ex){
       }finally{
       }       

   }

   public SubscribeClass  GetSubscribeByMACAddress(String mac)
   {

           var endPoint = new EndpointAddress(url);
           var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
           binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
           binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
           binding.MaxReceivedMessageSize = int.MaxValue;
           GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
           kr1Client.ClientCredentials.UserName.UserName = "root";
           kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
           SubscribeClass item = kr1Client.GetSubscribeByMACAddress(0, mac.ToLower());
           return item;
   }



   public void doSaveCur(String mac,String cds, int up, int down,String dhcp)
   {


       try
       {
           var endPoint = new EndpointAddress(url);
           var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
           binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
           binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
           binding.MaxReceivedMessageSize = int.MaxValue;
           GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
           kr1Client.ClientCredentials.UserName.UserName = "root";
           kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
           int CDS = Convert.ToInt32(cds);
       /*    if (GetSubscribeByMACAddress(mac) != null)
           {
             ResultClass rcx= kr1Client.UnSubscribe(0, mac);  
           }*/
                      
           DHCPItemClass[] item=null;
           if (dhcp == "1")
           {
                item = kr1Client.GetDHCPPool(enumAddressPoolType.PrivateAddressPool);
                dhcp = item[0].StartIP;
           }
           else
           {
               item = kr1Client.GetDHCPPool(enumAddressPoolType.PublicAddressPool);
               dhcp = item[0].StartIP;
           }

             ResultClass rc = kr1Client.AddSubscribe(0, mac, CDS, false, 200, up, down, false, enumAPI_Action.AllowPass, dhcp, enumAPI_OutWanIf.IfDefault);           
            // kr1Client.AllowPass(mac);
           //VLanGroupList
           //enumAPI_Action.

           //kr1Client.AddSubscribe
           
           
       }
       catch (Exception ex)
       {
       }
       finally
       {
       }

   }


   public void doSaveAll(int up,int down)
   {


       try
       {
           var endPoint = new EndpointAddress(url);
           var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
           binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
           binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
           binding.MaxReceivedMessageSize = int.MaxValue;
           GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
           kr1Client.ClientCredentials.UserName.UserName = "root";
           kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
           VLanDefineClass[] list = kr1Client.ListVLanDefine();

           foreach(VLanDefineClass item in list){
               kr1Client.SetVLanDefineBandwidthDown(item.VLanID, down);
               kr1Client.SetVLanDefineBandwidthUp(item.VLanID, up);
           }


       }
       catch (Exception ex)
       {
       }
       finally
       {
       }

   }  





}
}
