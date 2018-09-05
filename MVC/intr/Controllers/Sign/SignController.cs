using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Web.Http;
using IntranetSystem.Service;
using IntranetSystem.Models;
using IntranetSystem.Poco;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using IntranetSystem.Models.Service;
using IntranetSystem.Models.Edmx;
using System.IO;
using ZXing;                  // for BarcodeWriter
using ZXing.QrCode;
using System.Drawing;
using HowardCoupon_vs2010.Poco;           // for QRCode Engine

namespace IntranetSystem.Sign
{
    public class param
    {
        public string UUID { get; set; }
    }     

    public class SignController : Controller
    {
        QRCodeService qRCodeService = new QRCodeService();
        SignService signService = new SignService();
        CommonService commonService = new CommonService();
        //
        


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getSign(PagingParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = signService.getSign(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        [ValidateInput(false)]
        public ActionResult doCreate(SIGN param)
        {
            
            //var jsonObjList = JsonConvert.Deserialize<List<Dictionary<string, string>>>(ary);

            //JArray obj = JsonConvert.DeserializeObject<JArray>(ary);

          //  param.CONTENT =  JsonConvert.DeserializeObject<String>(param.CONTENT);

            var httpPostedFile = Request.Files["IMG"];
            
            JObject jo = new JObject();
            try
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string roleid = ticket.UserData;
                string userid = User.Identity.Name;
                param.EDIT_USER = userid;
                if (Request != null && httpPostedFile != null)
                {
                    upload(Request.Files["IMG"]);
                    param.IMG = Request.Files["IMG"].FileName;                    
                }




                int i = signService.doCreate(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }





        [ValidateInput(false)]
        public ActionResult doEdit(SIGN param)
        {
            //JArray obj = JsonConvert.DeserializeObject<JArray>(ary);
            //param.CONTENT = JsonConvert.DeserializeObject<String>(param.CONTENT);
            var httpPostedFile = Request.Files["IMG"];
            
            JObject jo = new JObject();
            try
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string roleid = ticket.UserData;
                string userid = User.Identity.Name;
                param.EDIT_USER = userid;

                if (Request != null && httpPostedFile!=null)
                {
                    upload(Request.Files["IMG"]);
                    param.IMG = Request.Files["IMG"].FileName;                    
                }





                int i = signService.doEdit(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public void upload(HttpPostedFileBase file)
        {
            string fileName="";
            string srcFile="";
            //有無上傳檔案
            if (file != null)
            {                
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                     fileName = Path.GetFileName(file.FileName);
                     srcFile = Path.Combine(Server.MapPath("~/Uploads/WEB"), fileName);
                    file.SaveAs(srcFile);
                    
                }
            }
            
            // SFTP上傳檔案
            if (!string.IsNullOrEmpty(fileName))
            {
                string destFile = "/var/www/html/HW2011/beta/sign/images/" + fileName;
                Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.10", "22", "root", "mka89444");
                sftp.Put(srcFile, destFile);
            }
        }






        public ActionResult doRemove(int UUID)
        {
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;           


            JObject jo = new JObject();
            try
            {

                int i = signService.doRemove(UUID);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        [ValidateInput(false)]
        public ActionResult doPrint(String CNAME, String ENAME, String TEL, String FAX, String MOBILE, String EMAIL, String REMARK2, String JOB, String DPT)
        {
            JObject jo = new JObject();

            var writer = new BarcodeWriter  //dll裡面可以看到屬性
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions //設定大小
                {
                    Height = 130,
                    Width = 130,
                    CharacterSet = "UTF-8"
                }
            };
            Bitmap img = writer.Write("MECARD:N:,"+CNAME+"("+ENAME+");TEL:"+TEL+";EMAIL:"+EMAIL+";NOTE:"+REMARK2+";;"); //轉QRcode的文字  
            //MECARD:N:,12213;TEL:12321323231123;EMAIL:123123;NOTE:123123;;
            String filename = EMAIL + ".png";
            String path = "c:\\temp\\" + filename;
            img.Save(path);
            
            string destFile = "/var/www/html/HW2011/beta/sign/sales/" + filename;
            Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.10", "22", "root", "mka89444");
            sftp.Put(path, destFile);
            String str = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=big5' /></head><body>" +
                        "<table width='500' border='0' cellpadding='0' style='font-size:11px;font-family:consolas ;font-weight: bold;'> "+
                        //"<tr><td height='45' colspan='3'>"+ JOB +"<br>" +DPT +" <hr style='border-top: 1px solid #000;'></td></tr><tr>"+
                        "<tr><td height='45' colspan='3'><span style='font-size:14px'>" + ENAME + "</span><br>" + JOB + "<br>" + DPT + "<hr style='display: block;margin-top: 0px;margin-bottom: 0px;border-style: inset;border-width: 1px;'></td></tr><tr>" +
                        "<td width='103' rowspan='7' valign='top'><img src='http://reservation.howard-hotels.com.tw/sign/sales/howard_logo.jpg' width='110' height='70'></td> " +
                        "<td width='112' rowspan='7' valign='top'><img src='http://reservation.howard-hotels.com.tw/sign/sales/" + EMAIL + ".png' width='100' height='100'></td>" +
                        "<td width='425'>TEL: +886 2 "+TEL+"</td> "+
                        "</tr><tr><td>FAX: +886 2 "+FAX+"</td></tr><tr><td>MOBILE: +886 "+MOBILE+"</td></tr><tr><td>E-MAIL:"+EMAIL+"</td> "+
                        "</tr> <tr><td>No.160 Ren Ai Road, Sec.3</td></tr><tr><td>Taipei, 10657, Taiwan </td></tr><tr>"+
                        "<td>http://www.howard-hotels.com.tw</td></tr></table></body></html>";
            jo.Add("CODE",str);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        
  


        public ActionResult getDepart()
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            ja = signService.getDepart();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult getDepartCurrent(int UUID)
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            ja = qRCodeService.getDepartCurrent(UUID);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
        
    }
}
