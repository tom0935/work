using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
using Jasper.service.frm3;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace Jasper.Controllers.frm3
{
    [Authorize]
    public class Frm301Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm301Service frm301Service = new Frm301Service();



        public ActionResult frm301()
        {
            return View();
        }


        public ActionResult getDG1(Frm301Poco param)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm301Service.getDG1(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


    




        public ActionResult doSave(Frm301Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm301Service.doSave(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm301Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm301Service.doAdd(param);

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
                int i = frm301Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult getINFTP()
        {
            JArray ja = new JArray();
            ja = commonService.getINFTP();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

    }

}
