using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace HowardCoupon_vs2010.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult getData(String test)
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            
            ja.Add("1");
            //return Content(JsonConvert.SerializeObject(ja), "application/json");
            String jsonp = "callback({status:1})";
            //return Content(JsonConvert.SerializeObject(jsonp), "application/json");
            return Content(jsonp);
        }


    }
}
