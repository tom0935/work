using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.service;
using Jasper.service.frm1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jasper.Controllers.frm1
{
    [Authorize]  
    public class Frm106Controller : Controller
    {
        Frm106Service f106service = new Frm106Service();
        //
        // GET: /frm106/

        public ActionResult frm106()
        {
            return View();
        }

        public ActionResult Grid(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            try
            {
            
                jo= f106service.getDatagrid( param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Grid2(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("rows", f106service.getDatagrid2(CNO));                
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save(String mode, BROKER param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f106service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Remove(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                int i = f106service.doRemove(CNO);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save2(String mode, BROKMAN param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f106service.doSave2(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Remove2(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = f106service.doRemove2(TAGID);
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
