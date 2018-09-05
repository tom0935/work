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
    public class Frm109Controller : Controller
    {
        Frm109Service f109service = new Frm109Service();
        //
        // GET: /frm109/

        public ActionResult frm109()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f109service.getTotalCount());
                jo.Add("rows", f109service.getDatagrid(page, rows, sort, order));
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
            jr = f109service.getCombobox();
            return Content(JsonConvert.SerializeObject(jr), "application/json");
        }


        public ActionResult Save(String mode, ACCF param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f109service.doSave(mode, param);
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
                int i = f109service.doRemove(TAGID);
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
