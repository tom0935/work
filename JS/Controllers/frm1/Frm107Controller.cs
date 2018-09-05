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
    public class Frm107Controller : Controller
    {
        Frm107Service f107service = new Frm107Service();
        //
        // GET: /frm107/

        public ActionResult frm107()
        {
            return View();
        }

      public ActionResult Grid(int page = 1, int rows = 20, String sort = "CNO", String order = "asc",String q="")
        {
            JObject jo = new JObject();
            try
            {
             //   jo.Add("total", f107service.getTotalCount());
                jo = f107service.getDatagrid(page, rows, sort, order,q);
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
                jo.Add("rows", f107service.getDatagrid2(CNO));                
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


      public ActionResult Save(List<Param> CNO,String mode, PBRENTAL param)
        {
            JObject jo = new JObject();
            try
            {
                int i = 0;
                int j = 0;
            if (CNO != null) {
                foreach (var obj in CNO)
                {
                    param.RMNO = obj.CNO;
                    j = f107service.doSave(mode, param);
                    i = i + j;
                }  
            }else{
                i = f107service.doSave(mode, param);
            }
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
                int i = f107service.doRemove(CNO);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getYR()
        {
            DateTime dt = DateTime.Now;
            JArray jr = new JArray();
            JObject jo = new JObject();
            int sy = 2003;//Convert.ToInt16(dt.Year);
            int ey = Convert.ToInt16(dt.Year + 5);
            for (int i = sy; i <= ey; i++)
            {                                
                var itemObject = new JObject
                    {                                           
                        {"ITEM",i.ToString()},
                        {"VALUE",i.ToString()}
                    };
                jr.Add(itemObject);
            }
            
            return Content(JsonConvert.SerializeObject(jr), "application/json");
        }


        [Serializable]  
        public class Param
        {
            public String CNO { get; set; }

            public String CNO2 { get; set; }
        }
    }
    
}
