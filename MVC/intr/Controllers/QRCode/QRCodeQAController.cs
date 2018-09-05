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
    public class QRCodeQAController : Controller
    {
        //
        // GET: /QRCodeQA/
        QRCodeQAService qRCodeQAService = new QRCodeQAService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getQRCodeQA(PagingParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = qRCodeQAService.getQRCodeQA(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getQRCodeQAReply(int UUID)
        {
            JObject jo = new JObject();
            jo = qRCodeQAService.getQRCodeQAReply(UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        [ValidateInput(false)]
        public ActionResult doReply(QRCODE_QA param)
        {
            
            JObject jo = new JObject();
            try
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string roleid = ticket.UserData;
                string userid = User.Identity.Name;
                param.RUSER = userid;
                String subject = param.CNAME +  " 貴賓您好:福華線上客服回覆信件";

                qRCodeQAService.SendEmail(subject, param.QUESTION,param.ANSWER, param.EMAIL,param.QRCODE_DTL_UUID);

                int i = qRCodeQAService.doReply(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


    }
}
