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
    public class PayTypeController : Controller
    {
        //
        // GET: /PayType/
        private PayTypeService payTypeService = new PayTypeService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getPayType(EasyuiParamPoco param,String CODE)
        {
            JObject jo = new JObject();
            jo = payTypeService.getPayType(param,CODE);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        public ActionResult doSave(DepartmentPoco param)
        {
            int i = 0;
            i=payTypeService.doSave(param);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = payTypeService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }


        public ActionResult chkCode(String CODE)
        {
            int i = payTypeService.chkCode(CODE);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

    }
}
