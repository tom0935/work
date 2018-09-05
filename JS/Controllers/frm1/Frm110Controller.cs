using Jasper.Models;
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
    public class Frm110Controller : Controller
    {
        Frm110Service f110service = new Frm110Service();
        //
        // GET: /frm110/

        public ActionResult frm110()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f110service.getTotalCount());
                jo.Add("rows", f110service.getDatagrid(page, rows, sort, order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult GridQuery(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f110service.getTotalCount2());
                jo.Add("rows", f110service.getDatagridQuery(page, rows, sort, order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getCountView()
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("view", f110service.getCountView());
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save(String mode, CARLOCT param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f110service.doSave(mode, param);
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
                int i = f110service.doRemove(CNO);
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
