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
    public class Frm112Controller : Controller
    {
        Frm112Service f112service = new Frm112Service();
        //
        // GET: /frm112/

        public ActionResult frm112()
        {
            return View();
        }

        public ActionResult Grid(EasyuiParamPoco param)
        {
            JObject jo = f112service.getDatagrid(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    



        public ActionResult Save(String mode, POSM param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f112service.doSave(mode, param);
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
                int i = f112service.doRemove(CNO);
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
