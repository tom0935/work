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
    public class DepartmentController : Controller
    {
        //
        // GET: /Department/
        private DepartmentService departmentService = new DepartmentService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getDepartment(EasyuiParamPoco param,String CODE)
        {
            JObject jo = new JObject();
            jo = departmentService.getDepartment(param,CODE);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    
        public ActionResult doSave(DepartmentPoco param)
        {
            int i = 0;
            i=departmentService.doSave(param);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = departmentService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }


        public ActionResult chkCode(String CODE)
        {
            int i = departmentService.chkCode(CODE);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

    }
}
