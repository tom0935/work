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
    public class PhrController : Controller
    {
        //
        // GET: /PayType/
        private PhrService phrService = new PhrService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getPhr(EasyuiParamPoco param,String UUID)
        {
            JObject jo = new JObject();
            jo = phrService.getPhr(param, UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        public ActionResult doSave(PhrPoco param)
        {
            int i = 0;
            i = phrService.doSave(param);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = phrService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        /*
        public ActionResult chkCode(String CODE)
        {
            int i = refTypeService.chkCode(CODE);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }
        */
    }
}
