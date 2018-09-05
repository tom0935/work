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



namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm201fController : Controller
    {

        Frm201fService frm201fService = new Frm201fService();
        CommonService commonService = new CommonService();

        public ActionResult frm201f()
        {
            return View();
        }

        //取回卡別列表 Combobox
        public ActionResult getCARDTYPE()
        {
            JArray ja = new JArray();
            ja = commonService.getCARDTYPE();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getCNM(String TAGID)
        {
            JObject jo = new JObject();
            jo = frm201fService.getCNM(TAGID);            
            return Content(JsonConvert.SerializeObject(jo), "application/json");

        }


        public ActionResult getDG1(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm201fService.getDG1(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }






        public ActionResult doSave(Frm201fPoco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm201fService.doSave(param);                
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm201fPoco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm201fService.doAdd( param);
                jo.Add("status", 1);
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
                int i = frm201fService.doRemove(TAGID);
                jo.Add("status",1);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    }        
  
}
