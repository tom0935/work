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
    public class Frm211Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm211Service frm211Service = new Frm211Service();



        public ActionResult frm211()
        {
            return View();
        }


        public ActionResult getDG1(String q)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm211Service.getDG1(q);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm211Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm211Service.doSave(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm211Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm211Service.doAdd(param);

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
                int i = frm211Service.doSuccess(TAGID);

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
                int i = frm211Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult getOTHERN()
        {
            JArray ja = new JArray();
            ja = commonService.getOTHERN();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

    }

}
