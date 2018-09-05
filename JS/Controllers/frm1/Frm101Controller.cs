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
    public class Frm101Controller : Controller
    {
    
        //
        // GET: /frm101/
        Frm101Service f101service = new Frm101Service();

        public ActionResult frm101()
        {
            return View();
        }

        public ActionResult Grid(EasyuiParamPoco param)
        {
            JObject jo = f101service.getDatagrid(param);                
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Dlg(String CNO)
        {            
            JObject jo = new JObject();
            jo.Add("rows", f101service.getDailog(CNO));
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Save(String mode,ROOMF param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f101service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch(Exception e) {
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
                int i = f101service.doRemove(CNO);
                jo.Add("status", i);
            }catch(Exception e){
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    }
}
