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
    public class Frm108Controller : Controller
    {
         Frm108Service f108service = new Frm108Service();
        //
        // GET: /frm108/

        public ActionResult frm108()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20, String sort = "TPNO", String order = "asc")
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("total", f108service.getTotalCount());
                jo.Add("rows", f108service.getDatagrid(page, rows,sort,order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult Save(String mode, ACCTP param)
        {
            JObject jo = new JObject();
            try
            {
                int i = f108service.doSave(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Remove(String TPNO)
        {
            JObject jo = new JObject();
            try
            {
                int i = f108service.doRemove(TPNO);
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
