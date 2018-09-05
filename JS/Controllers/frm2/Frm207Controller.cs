using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;



namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm207Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm207Service frm207Service = new Frm207Service();



        public ActionResult frm207()
        {
            return View();
        }


        public ActionResult getDG1(String RMNO,String FEEYM)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm207Service.getDG1(RMNO,FEEYM);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm207Poco param)
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
                int i = frm207Service.doSave(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm207Poco param)
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
                int i = frm207Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSuccess(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm207Service.doSuccess(TAGID);

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
                int i = frm207Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回雜費項目列表 Combobox
        public ActionResult getFEETP()
        {
            JArray ja = new JArray();
            ja = commonService.getFEETP();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


    }

}
