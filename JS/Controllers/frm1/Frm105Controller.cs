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
    public class Frm105Controller : Controller
    {
        Frm105Service f105service = new Frm105Service();
        //
        // GET: /frm105/

        public ActionResult frm105()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            jo = f105service.getDatagrid(page, rows, sort, order);


            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Grid2(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("rows", f105service.getDatagrid2(CNO));
                jo.Add("footer", f105service.getDatagrid2Sum(CNO));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save(String mode, EQUIP param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f105service.doSave(mode, param);
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
                int i = f105service.doRemove(CNO);
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
