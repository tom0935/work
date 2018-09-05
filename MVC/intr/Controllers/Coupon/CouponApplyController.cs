using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using IntranetSystem.Models.Service;
using Newtonsoft.Json;
using IntranetSystem.Poco;
using IntranetSystem.Models.Edmx;

namespace IntranetSystem.Controllers.QRCode
{
    public class CouponApplyController : Controller
    {
        //
        // GET: /QRCodeQA/
        CouponApplyService couponApplyService = new CouponApplyService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult doApply(CouponITFParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;

           
                jo = couponApplyService.doApply(param,userid);                                        
             //   jo.Add("status", jo);
            
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getApply(PagingParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = couponApplyService.getApply(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



    }
}
