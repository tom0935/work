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
    public class CompanyController : Controller
    {
        //
        // GET: /Company/
        private CompanyService companyService = new CompanyService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getCompany(EasyuiParamPoco param,String CODE)
        {
            JObject jo = new JObject();
            jo = companyService.getCompany(param,CODE);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        public ActionResult doSave(CompanyPoco param)
        {
            int i = 0;
            i=companyService.doSave(param);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = companyService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult chkCode(String CODE)
        {            
            int i=companyService.chkCode(CODE);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

    }
}
