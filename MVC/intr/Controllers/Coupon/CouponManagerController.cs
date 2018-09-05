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

namespace IntranetSystem.Controllers
{
    public class CouponManagerController : Controller
    {
        //
        // GET: /QRCodeQA/
        CouponManagerService couponManagerService = new CouponManagerService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult doSave(String ISUID,String CINO)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = couponManagerService.doSave(ISUID,CINO);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doSave1(String ISUID, String CINO)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = couponManagerService.doSave1(ISUID, CINO);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doQuery(String CINO)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            ja = couponManagerService.getCi(CINO);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }



        public ActionResult doUpdate4(CouponITFParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;

           
                jo = couponManagerService.doApply(param,userid);                                        
             //   jo.Add("status", jo);
            
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



    }
}
