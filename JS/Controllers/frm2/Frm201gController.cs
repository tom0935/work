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
    public class Frm201gController : Controller
    {

        Frm201gService frm201gService = new Frm201gService();


        public ActionResult frm201g()
        {
            return View();
        }


        public ActionResult getDG1(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm201gService.getDG1(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getDG2()
        {
            JObject jo = new JObject();
            try
            {
                jo = frm201gService.getDG2();
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm201ePoco param,List<Frm201ePoco> LIST)
        {
            JObject jo = new JObject();
            try
            {
               // int i = frm201eService.doSave(param, LIST);
                int i = 0;
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm201gPoco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm201gService.doAdd( param);                
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doRemove(String JOBTAG, String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm201gService.doRemove(JOBTAG,TAGID);                
                jo.Add("status", i);
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
