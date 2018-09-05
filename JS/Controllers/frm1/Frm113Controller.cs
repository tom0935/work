using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
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
    public class Frm113Controller : Controller
    {
        Frm113Service f113service = new Frm113Service();
        CommonService cs = new CommonService();
        //
        // GET: /frm113/

        public ActionResult frm113()
        {
            return View();
        }

        public ActionResult Grid(EasyuiParamPoco param)
        {
            JObject jo = f113service.getDatagrid(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }





        public ActionResult Save(String mode, HOMEAST param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f113service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult Remove(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = f113service.doRemove(TAGID);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCOUNTRY()
        {            
            JArray ja = cs.getCOUNTRY();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }




    }
}
