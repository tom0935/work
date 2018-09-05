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
    public class Frm104Controller : Controller
    {
        Frm104Service f104service = new Frm104Service();
        //
        // GET: /frm104/

        public ActionResult frm104()
        {
            return View();
        }
        public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f104service.getTotalCount());
                jo.Add("rows", f104service.getDatagrid(page, rows,sort,order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult Save(String mode, COUNTRY param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f104service.doSave(mode, param);
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
                int i = f104service.doRemove(CNO);
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
