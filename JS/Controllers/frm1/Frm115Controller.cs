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
    public class Frm115Controller : Controller
    {
        Frm115Service f115service = new Frm115Service();
        //
        // GET: /frm115/

        public ActionResult frm115()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f115service.getTotalCount());
                jo.Add("rows", f115service.getDatagrid(page, rows, sort, order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Combobox()
        {
            DateTime dt = DateTime.Now;
            JArray jr = new JArray();
            JObject jo = new JObject();
            jr = f115service.getCombobox();
            return Content(JsonConvert.SerializeObject(jr), "application/json");
        }


        public ActionResult Save(String mode, EMPLOYEE param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f115service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Remove(int UUID)
        {
            JObject jo = new JObject();
            try
            {
                int i = f115service.doRemove(UUID);
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
