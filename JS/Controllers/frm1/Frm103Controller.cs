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
    public class Frm103Controller : Controller
    {
        Frm103Service f103service = new Frm103Service();
        // GET: /frm103/

        public ActionResult frm103()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f103service.getTotalCount());
                jo.Add("rows", f103service.getDatagrid(page, rows,sort,order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Grid2(String CNO)
        {
            JObject jo = new JObject();
            try
            {                
                jo.Add("rows", f103service.getDatagrid2(CNO));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult Save(String mode, PUBLISHER param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f103service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save2(String mode, PUBLISHF param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f103service.doSave2(mode, param);
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
                int i = f103service.doRemove(CNO);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Remove2(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = f103service.doRemove2(TAGID);
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
