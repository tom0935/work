using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HowardCoupon.Poco;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HowardCoupon_vs2010.Models.Service;
using System.Net;


namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class CusController : Controller
    {
        //
        // GET: /Cus/
        private CusService cusService = new CusService();
        public ActionResult Index(String CMPID,String UUID)
        {
            if (CMPID != null)
            {
                ViewBag.CMPID = CMPID;
            }
            else if (UUID != null)
            {
                ViewBag.UUID = UUID;
            }
            return View();
        }

        public ActionResult getCus(EasyuiParamPoco param,String UUID,String SEARCH,String CMPID)
        {
            JObject jo = new JObject();    
            jo = cusService.getCus(param, UUID,SEARCH,CMPID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        public ActionResult doSave(CusPoco param)
        {

            int i = 0;
            i = 0;            
             i = cusService.doSave(param);            
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = cusService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult chkCode(String CMPID)
        {            
            int i=cusService.chkCode(CMPID);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }


        public ActionResult getCity(String ZIP)
        {
            JArray ja = new JArray();            
            ja = cusService.getCity(ZIP);            
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getCityList()
        {
            JArray ja = new JArray();
            ja = cusService.getCity();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getZIPList(String NAME)
        {
            JArray ja = cusService.getZIPList(NAME);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult getZIP(String ZIP)
        {
            JArray ja = cusService.getZIP(ZIP);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
    }
}
