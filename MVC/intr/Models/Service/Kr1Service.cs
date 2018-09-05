
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


using System.Xml;



namespace IntranetSystem.Service
{
    public class VLanClass
    {
        public string VLAN { get; set; }
    }

    public class Kr1Service
{

    private Entities db = new Entities();
    private GatewayAPISoapClient kr1Client;
    private CommonService commonService = new CommonService();
    private const String url = "http://122.147.252.5:88/API/GatewayAPI.asmx";
  
    public JObject getVLanDatagrid(EasyuiParamPoco param) 
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
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
            List<VLanDefineClass> list =new List<VLanDefineClass>();
            foreach (VLanDefineClass item in VLanDefineClass)
            {
                list.Add(item);
            }
            var query = (from t in list select t).AsQueryable();
            
            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序                    
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    

            
            foreach ( var item in query)
            {
                var itemObject = new JObject
                    {                                           
                        {"VLanID",StringUtils.getString(item.VLanID)},
                        {"VLanState",StringUtils.getString(item.VLanState)}, 
                        {"ActionType",StringUtils.getString(item.ActionType)}, 
                        {"DHCPStartIP",StringUtils.getString(item.DHCPStartIP)}, 
                        {"DisableLogin",StringUtils.getString(item.DisableLogin)}, 
                        {"BandwidthUp",StringUtils.getString(item.BandwidthUp)}, 
                        {"BandwidthDown",StringUtils.getString(item.BandwidthDown)}                        
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", list.Count());
        }catch(Exception ex){
        }finally{
           // kr1Client.Close();
        }
        

    
       return jo;
    }




   public void initVLan_Schedule(){
       doCreateVLanDB();
   }


   public JObject getVLanDBDatagrid(EasyuiParamPoco param)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
        try
        {
            var cnt = (from t in db.VLAN_SCHEDULE select t).Count();
            if (cnt == 0)
            {
                initVLan_Schedule();
            }
            var query = from t in db.VLAN_SCHEDULE select t;           

            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序                    
          //  query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    


            foreach (var item in query)
            {

                String sdt = "";
                String edt = "";
                String stime="";
                String etime="";



                if (item.SDT != null)
                {
                    var dt = (DateTime)item.SDT;
                    sdt = dt.ToString("yyyy-MM-dd");
                    stime=dt.ToString("HH");
                }
                if (item.EDT != null)
                {
                    var dt = (DateTime)item.EDT;
                    edt = dt.ToString("yyyy-MM-dd");
                    etime=dt.ToString("HH");
                }
                String sch="";




                

                if (StringUtils.getString(item.SCHED) == "Y")
                {
                    sch = "";
                }

                if (sdt != "" && edt != "")
                {
                    DateTime sdt1 = DateTimeUtil.getDateTime(sdt + " " + stime + ":00:00");
                    DateTime edt1 = DateTimeUtil.getDateTime(edt + " " + etime + ":00:00");

                    if (DateTime.Compare(System.DateTime.Now, sdt1) > 0 && DateTime.Compare(System.DateTime.Now, edt1) <= 0)
                    {
                        sch = "計劃中";
                    }
                }

                var itemObject = new JObject
                    {                                           
                        {"VLAN",StringUtils.getString(item.VLAN)},                        
                        {"SDT",sdt},                        
                        {"EDT",edt},
                        {"STIME",stime},
                        {"ETIME",etime},
                        {"SESSION",StringUtils.getString(item.SESSION_CNT)},
                        {"DHCPIP",StringUtils.getString(item.DHCPIP)}, 
                        {"BandwidthUp",StringUtils.getString(item.UP)}, 
                        {"BandwidthDown",StringUtils.getString(item.DOWN)}, 
                        {"SCHED",sch},
                        {"RC",item.RC},
                        {"ACTION",item.ACTION}
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", query.Count());
        }
        catch (Exception ex)
        {
        }
        finally
        {
            // kr1Client.Close();
        }



        return jo;
    }



    public int doCreateVLanDB()
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
                    obj.ACTION = item.ActionType.ToString();
                    
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




    public JObject getSessionDatagrid(EasyuiParamPoco param)
    {
        JArray ja = new JArray();
        JObject jo = new JObject();
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
            Array ConcurrentClass = kr1Client.GetConcurrent();
            Array DHCPAllocListClass = kr1Client.GetDHCPAllocList();
            List<ConcurrentClass> list = new List<ConcurrentClass>();

            List<DHCPAllocItem> dhcplist = new List<DHCPAllocItem>();


            foreach (ConcurrentClass item in ConcurrentClass)
            {
                list.Add(item);/*
                foreach (DHCPAllocItem dhcp in DHCPAllocListClass)
                {
                    if (item.MAC.ToUpper() == dhcp.MACAddr.ToUpper())
                    {
                        item.OutWanIf = dhcp.HostName;
                    }
                }*/
             
            }
/*
            foreach (DHCPAllocItem item in DHCPAllocListClass)
            {
                dhcplist.Add(item);
            }
            */
            var query = (from t in list select t).AsQueryable();

            query = query.OrderBy(string.Format("{0} {1}", param.sort, param.order)); //排序                    
            query = query.Skip((param.page - 1) * param.rows).Take(param.rows);    //分頁    


            foreach (var item in query)
            {
                var itemObject = new JObject
                    {                                           
                        {"VLanID",StringUtils.getString(item.VLanID)}, 
                        {"MAC",StringUtils.getString(item.MAC)},                         
                        {"IP",StringUtils.getString(item.IP)}, 
                        {"State",StringUtils.getString(item.State)},                                                
                        {"CurrentFlowUp",StringUtils.getString(item.CurrentFlowUp)}, 
                        {"CurrentFlowDown",StringUtils.getString(item.CurrentFlowDown)}, 
                        {"Bandwidth_Up",StringUtils.getString(item.Bandwidth_Up)}, 
                        {"Bandwidth_Down",StringUtils.getString(item.Bandwidth_Down)}, 
                        {"TotalFlowUpByte",StringUtils.getString(item.TotalFlowUpByte)}, 
                        {"TotalFlowDownByte",StringUtils.getString(item.TotalFlowDownByte)}, 
                        {"LoginDate",StringUtils.getString(item.LoginDate)}, 
                        {"IdleTime",StringUtils.getString(item.IdleTime)}, 
                        {"ExpireDate",StringUtils.getString(item.ExpireDate)}, 
                        {"HostName",StringUtils.getString(item.OutWanIf)}
                    };
                ja.Add(itemObject);
            }
            jo.Add("rows", ja);
            jo.Add("total", list.Count());
        }
        catch (Exception ex)
        {
        }
        finally
        {
            // kr1Client.Close();
        }



        return jo;
    }

    public int doSaveDB(int VLAN, String SDT,String STIME, String EDT,String ETIME, int UP, int DOWN, String DHCPIP,String ACTION,String userid)
    {
        int i = 0;
        String dhcp = "";
        try
        {
            using (db)
            {
                var endPoint = new EndpointAddress(url);
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.MaxReceivedMessageSize = int.MaxValue;
                GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
                kr1Client.ClientCredentials.UserName.UserName = "root";
                kr1Client.ClientCredentials.UserName.Password = "spadmin2075";
                
                VLAN_SCHEDULE obj = (from t in db.VLAN_SCHEDULE where t.VLAN == VLAN select t).SingleOrDefault();
                if (obj != null)
                {
                    DateTime sdt = DateTimeUtil.getDateTime(SDT + " "+ STIME +":00:00");
                    DateTime edt = DateTimeUtil.getDateTime(EDT + " " + ETIME + ":00:00");

                    if (DHCPIP != obj.DHCPIP)
                    {
                        if (StringUtils.getString(DHCPIP) == "1")
                        {
                            dhcp = getPrivateDhcp();
                        }
                        else
                        {
                            dhcp = getPublicDhcp();
                        }
                    }
                    else
                    {
                        dhcp = obj.DHCPIP;
                    }


                    if (DateTime.Compare(System.DateTime.Now, sdt) > 0 && DateTime.Compare(System.DateTime.Now, edt) <= 0)
                    {



                        if (UP != Convert.ToInt32(obj.UP))
                        {
                            kr1Client.SetVLanDefineBandwidthUp(VLAN, UP);
                        }

                        if (DOWN != Convert.ToInt32(obj.DOWN))
                        {
                            kr1Client.SetVLanDefineBandwidthDown(VLAN, DOWN);
                        }

                            kr1Client.SetVLanDefineDHCPIP(VLAN, dhcp);

                        if (ACTION == "Charge")
                        {
                            kr1Client.SetVLanDefineAction(VLAN, enumVLanActionType.Charge);
                        }
                        else
                        {
                            kr1Client.SetVLanDefineAction(VLAN, enumVLanActionType.Free);
                        }

                        obj.SCHED = "Y";
                        obj.SDT = sdt;
                        obj.EDT = edt;
                        
                        
                    }
                    else if (DateTime.Compare(System.DateTime.Now, edt) > 0)
                    {
                        obj.SCHED = "Y";
                        if (UP != Convert.ToInt32(obj.DF_UP))
                        {
                            kr1Client.SetVLanDefineBandwidthUp(VLAN,Convert.ToInt32(obj.DF_UP));
                        }

                        if (DOWN != Convert.ToInt32(obj.DF_DOWN))
                        {
                            kr1Client.SetVLanDefineBandwidthDown(VLAN, Convert.ToInt32(obj.DF_DOWN));
                        }
                        DOWN = Convert.ToInt32(obj.DF_DOWN);
                        UP = Convert.ToInt32(obj.DF_UP);
                        dhcp = obj.DF_DHCPIP;
                        obj.SDT = null;
                        obj.EDT = null;

                        kr1Client.SetVLanDefineAction(VLAN, enumVLanActionType.Charge);
                        kr1Client.SetVLanDefineDHCPIP(VLAN, obj.DF_DHCPIP);
                    }
                    else
                    {
                        obj.SDT = sdt;
                        obj.EDT = edt;                        
                        obj.SCHED = "N";
                    }

                    obj.DHCPIP = dhcp;
                    obj.DOWN = DOWN;
                    obj.UP = UP;
                    obj.UPD_UID = userid;
                    obj.UPD_DT = System.DateTime.Now;
                    obj.ACTION = ACTION;
                    i= db.SaveChanges();                    
                }
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


    public int doSaveDB(List<VLanClass> LIST, String SDT, String STIME, String EDT, String ETIME, int UP, int DOWN, String DHCPIP,String ACTION, String userid)
    {
        int i = 0;
        String dhcp = "";
        try
        {
            using (db)
            {
                var endPoint = new EndpointAddress(url);
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.MaxReceivedMessageSize = int.MaxValue;
                GatewayAPISoapClient kr1Client = new GatewayAPISoapClient(binding, endPoint);
                kr1Client.ClientCredentials.UserName.UserName = "root";
                kr1Client.ClientCredentials.UserName.Password = "spadmin2075";

                List<decimal> code = new List<decimal>();
                foreach (VLanClass item in LIST)
                {
                    code.Add( StringUtils.getDecimal(item.VLAN));
                }

                var query = (from t in db.VLAN_SCHEDULE where code.Contains(t.VLAN) select t);

                foreach (var obj in query)
                {
                    DateTime sdt = DateTimeUtil.getDateTime(SDT + " " + STIME + ":00:00");
                    DateTime edt = DateTimeUtil.getDateTime(EDT + " " + ETIME + ":00:00");

                    if (DHCPIP != obj.DHCPIP)
                    {
                        if (StringUtils.getString(DHCPIP) == "1")
                        {
                            dhcp = getPrivateDhcp();
                        }
                        else
                        {
                            dhcp = getPublicDhcp();
                        }
                    }
                    else
                    {
                        dhcp = obj.DHCPIP;
                    }


                    if (DateTime.Compare(System.DateTime.Now, sdt) > 0 && DateTime.Compare(System.DateTime.Now, edt) <= 0)
                    {



                        if (UP != Convert.ToInt32(obj.UP))
                        {
                            kr1Client.SetVLanDefineBandwidthUp(Convert.ToInt32(obj.VLAN), UP);
                        }

                        if (DOWN != Convert.ToInt32(obj.DOWN))
                        {
                            kr1Client.SetVLanDefineBandwidthDown(Convert.ToInt32(obj.VLAN), DOWN);
                        }

                        kr1Client.SetVLanDefineDHCPIP(Convert.ToInt32(obj.VLAN), dhcp);
                        if (ACTION == "Charge")
                        {
                            kr1Client.SetVLanDefineAction(Convert.ToInt32(obj.VLAN), enumVLanActionType.Charge);
                        }
                        else
                        {
                            kr1Client.SetVLanDefineAction(Convert.ToInt32(obj.VLAN), enumVLanActionType.Free);
                        }                        
                        obj.SCHED = "Y";
                        obj.SDT = sdt;
                        obj.EDT = edt;
                        

                    }
                    else if (DateTime.Compare(System.DateTime.Now, edt) > 0)
                    {
                        obj.SCHED = "Y";
                        if (UP != Convert.ToInt32(obj.DF_UP))
                        {
                            kr1Client.SetVLanDefineBandwidthUp(Convert.ToInt32(obj.VLAN), Convert.ToInt32(obj.DF_UP));
                        }

                        if (DOWN != Convert.ToInt32(obj.DF_DOWN))
                        {
                            kr1Client.SetVLanDefineBandwidthDown(Convert.ToInt32(obj.VLAN), Convert.ToInt32(obj.DF_DOWN));
                        }
                        kr1Client.SetVLanDefineAction(Convert.ToInt32(obj.VLAN), enumVLanActionType.Charge);

                        DOWN = Convert.ToInt32(obj.DF_DOWN);
                        UP = Convert.ToInt32(obj.DF_UP);
                        dhcp = obj.DF_DHCPIP;
                        obj.SDT = null;
                        obj.EDT = null;
                        obj.ACTION = "Charge";
                        kr1Client.SetVLanDefineDHCPIP(Convert.ToInt32(obj.VLAN), obj.DF_DHCPIP);
                    }
                    else
                    {
                        obj.SDT = sdt;
                        obj.EDT = edt;

                        obj.SCHED = "N";
                    }

                    obj.DHCPIP = dhcp;
                    obj.DOWN = DOWN;
                    obj.UP = UP;
                    obj.UPD_UID = userid;
                    obj.UPD_DT = System.DateTime.Now;
                    obj.ACTION = ACTION;
                }
                i = db.SaveChanges();
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

   public XmlTextWriter doExport(XmlTextWriter XTW, String Account, String Password)
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
      List<VLanDefineClass> list = new List<VLanDefineClass>();
      XTW.WriteStartDocument(); 
      XTW.WriteStartElement("ArrayOfVLanDefineClass");
      XTW.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
      XTW.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
      
      foreach (VLanDefineClass item in VLanDefineClass)
      {
          XTW.WriteStartElement("VLanDefineClass");
          XTW.WriteElementString("DisableLogin", "true");         
          XTW.WriteElementString("VLanID", item.VLanID.ToString());        
          XTW.WriteElementString("RoomID", item.RoomID);        
          XTW.WriteElementString("LoginAccount", Account);          
          XTW.WriteElementString("LoginPassword", Password);          
          XTW.WriteElementString("DHCPStartIP", item.DHCPStartIP);          
          XTW.WriteElementString("ActionType", "Charge");          
          XTW.WriteElementString("SessionCountMax", item.SessionCountMax.ToString());          
          XTW.WriteElementString("BandwidthUp", item.BandwidthUp.ToString());          
          XTW.WriteElementString("BandwidthDown", item.BandwidthDown.ToString());          
          XTW.WriteElementString("IsSMTPRedir", "false");          
          XTW.WriteElementString("ChargePlanID", "1");          
          XTW.WriteElementString("PayType", "AskUser");          
          XTW.WriteElementString("VLanGroupID", "0");          
          XTW.WriteElementString("OutWanIf", "");          
          XTW.WriteElementString("VLanState", "false");
          XTW.WriteEndElement();
      }
      
      XTW.WriteEndElement();
      XTW.WriteEndDocument(); 
      return XTW;
   }



}
}
