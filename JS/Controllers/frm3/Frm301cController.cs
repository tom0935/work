using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
using Jasper.service.frm3;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace Jasper.Controllers.frm3
{
    [Authorize]
    public class Frm301cController : Controller
    {
        Frm201Service f201service = new Frm201Service();
        Frm107Service f107service = new Frm107Service();
        CommonService commonService = new CommonService();
        //
        CustomerService2 customerService2 = new CustomerService2();


        public ActionResult frm201c()
        {
            return View();
        }




        //取回國籍列表 Combobox
        public ActionResult getCNTRY()
        {
            JArray ja = new JArray();
            ja = commonService.getCOUNTRY();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回關係列表 Combobox
        public ActionResult getAPPE()
        {
            JArray ja = new JArray();
            ja = commonService.getAPPE();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回專線列表
        public ActionResult getTELCODE()
        {
            JArray ja = new JArray();
            ja = commonService.getTELCODE();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回使用者列表
        public ActionResult getOWNER(String TAGID,String CSTA)
        {
            JArray ja = new JArray();
            ja = customerService2.getOWNER(TAGID, CSTA);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getOWNER2(String TAGID,String CSTA)
        {
            JArray ja = new JArray();
            ja = customerService2.getOWNER2(TAGID,CSTA);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回車位編號
        public ActionResult getPARKNO(String TAGID,String CSTA)
        {
            JArray ja = new JArray();
            ja = customerService2.getPARKNO(TAGID,CSTA);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回幫傭姓名
        public ActionResult getASST(String TAGID,String CSTA)
        {
            JArray ja = new JArray();
            ja = customerService2.getASST(TAGID,CSTA);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getCusTab1DG(String TAGID,String CSTA)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService2.getCusTab1DG(TAGID,CSTA);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCusTab2DG(String TAGID,String CSTA)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService2.getCusTab2DG(TAGID,CSTA);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCusTab3DG(String TAGID,String CSTA)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService2.getCusTab3DG(TAGID,CSTA);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCusTab4DG(String TAGID,String CSTA)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService2.getCusTab4DG(TAGID,CSTA);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doSave(Frm201cPoco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = customerService2.doSave(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doRemove(String TYPE, String TAGID, String CODT)
        {
            JObject jo = new JObject();
            try
            {
                int i = customerService2.doRemove(TYPE,TAGID,CODT);
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
