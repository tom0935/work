using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Web.Http;
using IntranetSystem.Models;
using IntranetSystem.Service;

namespace IntranetSystem.Controllers.Cake
{

    public class CakeClassCodeController : Controller
    {
        //
        // GET: /frm101/
        CakeOrdersService cakeOrdersService = new CakeOrdersService();

        public ActionResult CakeClassCode()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, int rows = 20,String sort="CNO",String order="asc")
        {            
            JObject jo = new JObject();
            try
            {
              //  jo.Add("total", cakeOrdersService.getTotalCount());
             //   jo.Add("rows", cakeOrdersService.getDatagrid(page, rows, sort, order));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        /*
        public ActionResult Dlg(String CNO)
        {            
            JObject jo = new JObject();
            jo.Add("rows", cakeOrdersService.getDailog(CNO));
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }*/

        public ActionResult Save(String mode, CAKE_ORDERS param)
        {
            JObject jo = new JObject();
            try
            {
               // int i = cakeOrdersService.doSave(mode, param);
                jo.Add("status", 0);
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
                int i = cakeOrdersService.doRemove(CNO);
                jo.Add("status", i);
            }catch(Exception e){
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



    }
}
