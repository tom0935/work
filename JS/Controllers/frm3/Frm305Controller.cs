using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
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
    public class Frm305Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm305Service frm305Service = new Frm305Service();



        public ActionResult frm305()
        {
            return View();
        }


        public ActionResult getDG1()
        {
            JObject jo = new JObject();
            try
            {
                jo = frm305Service.getDG1();
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm305Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm305Service.doSave(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm305Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm305Service.doAdd(param);

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
