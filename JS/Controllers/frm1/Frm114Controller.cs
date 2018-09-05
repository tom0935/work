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
    public class Frm114Controller : Controller
    {
        Frm114Service f114service = new Frm114Service();
        //
        // GET: /frm114/

        public ActionResult frm114()
        {
            return View();
        }

        public ActionResult Grid(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            try
            {
                jo= f114service.getDatagrid(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Grid2(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("rows", f114service.getDatagrid2(CNO));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult Save(String mode, MAKR param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f114service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save2(String mode, MAKRMAN param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f114service.doSave2(mode, param);
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
                int i = f114service.doRemove(CNO);
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
                int i = f114service.doRemove2(TAGID);
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
