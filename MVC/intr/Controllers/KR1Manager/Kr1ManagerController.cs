using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IntranetSystem.Service;
using IntranetSystem.Poco;
using System.Web.Security;
using System.Xml;
using System.Text;
using IntranetSystem.Models.Common;




namespace IntranetSystem.Controllers.KR1Manager
{
    public class Kr1ManagerController : Controller
    {


        //
        // GET: /Kr1Manager/
        private Kr1Service service = new Kr1Service();


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Vlan()
        {
            return View();
        }

        public ActionResult VlanSch()
        {
            return View();
        }


        public ActionResult doCreateVLanDB()
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = service.doCreateVLanDB();
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult VLanScheduleGrid(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                jo = service.getVLanDBDatagrid(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult VLanGrid(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                jo = service.getVLanDatagrid(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult SessionGrid(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                jo = service.getSessionDatagrid(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getListVLanDefineIP(String vlanid,String mac)
        {
            JObject jo = service.getListVLanDefineIP(vlanid,mac);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doSaveDB(int VLAN, String SDT, String STIME, String EDT, String ETIME, int UP, int DOWN, String DHCPIP,String ACTION)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            try
            {
                int i = service.doSaveDB(VLAN, SDT,STIME, EDT,ETIME,UP,DOWN,DHCPIP,ACTION,userid);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doSaveDBArray(List<VLanClass> LIST, String SDT, String STIME, String EDT, String ETIME, int UP, int DOWN, String DHCPIP,String ACTION)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            int i = 0;
            try
            {
                
                i = service.doSaveDB(LIST, SDT, STIME, EDT, ETIME, UP, DOWN, DHCPIP,ACTION, userid);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doReSet(int VLAN)
        {
            JObject jo = new JObject();
            try
            {
                int i =service.doReSet(VLAN);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getDHCP()
        {
    
            JArray ja = new JArray();
            try
            {
                ja = service.getDhcpPool();
             
            }
            catch (Exception e)
            {
            }

            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult SessionDelete(String ip)
        {
            JObject jo = new JObject();
            try
            {
                service.SessionDelete(ip);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doSave(int vid,int up,int down)
        {
            JObject jo = new JObject();
            try
            {
                service.doSave(vid, up, down);
                jo.Add("status", "1");
            }catch(Exception e){
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doSaveCur(String mac,String cds, int up, int down,String dhcp)
        {
            JObject jo = new JObject();
            try
            {
                service.doSaveCur( mac, cds, up, down, dhcp);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doSaveAll(int up, int down)
        {
            JObject jo = new JObject();
            try
            {
                service.doSaveAll(up,down);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doExport(String ACCOUNT,String PASSWORD)
        {
            StringWriterWithEncoding writer = new StringWriterWithEncoding(Encoding.UTF8);
            XmlTextWriter XTW = new XmlTextWriter(writer);
            XTW = service.doExport(XTW,ACCOUNT, PASSWORD);            
            XTW.Flush();
            XTW.Close();
            byte[] data = Encoding.UTF8.GetBytes(writer.ToString());
            return File(data, "text/xml", "VLanConfig.xml");           
        }


    }
}
