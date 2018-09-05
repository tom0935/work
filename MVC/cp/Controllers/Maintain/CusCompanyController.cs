using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HowardCoupon.Poco;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HowardCoupon_vs2010.Models.Service;



namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class CusCompanyController : Controller
    {
        //
        // GET: /Company/
        private CusCompanyService cusCompanyService = new CusCompanyService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getCusCompany(EasyuiParamPoco param,String CMPID,String SEARCH)
        {
            JObject jo = new JObject();
            jo = cusCompanyService.getCusCompany(param, CMPID,SEARCH);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        public ActionResult doSave(CusCompanyPoco param)
        {

            int i = 0;
            i = 0;            
             i = cusCompanyService.doSave(param);            
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = cusCompanyService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult chkCode(String CMPID)
        {            
            int i=cusCompanyService.chkCode(CMPID);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }


        public ActionResult getCity(String ZIP)
        {
            JArray ja = new JArray();            
            ja = cusCompanyService.getCity(ZIP);            
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getCityList()
        {
            JArray ja = new JArray();
            ja = cusCompanyService.getCity();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getZIPList(String NAME)
        {
            JArray ja = cusCompanyService.getZIPList(NAME);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult getZIP(String ZIP)
        {
            JArray ja = cusCompanyService.getZIP(ZIP);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
    }
}
