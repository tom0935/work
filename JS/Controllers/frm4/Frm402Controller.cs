using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
using Jasper.service.frm4;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;



namespace Jasper.Controllers.frm4
{
    [Authorize]
    public class Frm402Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm402Service frm402Service = new Frm402Service();



        public ActionResult frm402()
        {
            return View();
        }


        public ActionResult getDG1(String q)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm402Service.getDG1(q);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm402Poco param)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                if (StringUtils.getString(param.LOGUSR) == "")
                {
                    param.LOGUSR = userid;
                }
                int i = frm402Service.doSave(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm402Poco param)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                if (StringUtils.getString(param.LOGUSR) == "")
                {
                    param.LOGUSR = userid;
                }
                int i = frm402Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }






        public ActionResult doRemove(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm402Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult getTM()
        {
            JArray ja = new JArray();
            ja = frm402Service.getTM();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult getCLASNM()
        {
            JArray ja = new JArray();
            ja = frm402Service.getCLASNM();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getTEACHER(String CNO)
        {
            JArray ja = new JArray();
            ja = frm402Service.getTEACHER(CNO);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

    }

}
