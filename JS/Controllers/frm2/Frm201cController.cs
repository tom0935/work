using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm201cController : Controller
    {
        Frm201Service f201service = new Frm201Service();
        Frm107Service f107service = new Frm107Service();
        CommonService commonService = new CommonService();
        //
        CustomerService customerService = new CustomerService();


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
        public ActionResult getOWNER(String TAGID)
        {
            JArray ja = new JArray();
            ja = customerService.getOWNER(TAGID);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getOWNER2(String TAGID)
        {
            JArray ja = new JArray();
            ja = customerService.getOWNER2(TAGID);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getOWNER2MORE(String TAGID,String CNO)
        {
            JArray ja = new JArray();
            ja = customerService.getOWNER2MORE(TAGID,CNO);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回車位編號
        public ActionResult getPARKNO(String TAGID)
        {
            JArray ja = new JArray();
            ja = customerService.getPARKNO(TAGID);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回幫傭姓名
        public ActionResult getASST(String TAGID)
        {
            JArray ja = new JArray();
            ja = customerService.getASST(TAGID);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getCusTab1DG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService.getCusTab1DG(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCusTab2DG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService.getCusTab2DG(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCusTab3DG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService.getCusTab3DG(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCusTab4DG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = customerService.getCusTab4DG(TAGID);
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
                int i = customerService.doSave(param);
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
                int i = customerService.doRemove(TYPE,TAGID,CODT);
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
