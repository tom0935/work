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
using System.Drawing;           // for QRCode Engine

namespace IntranetSystem.QRCodeView
{
    public class param
    {
        public string UUID { get; set; }
    }     

    public class QRCodeViewController : Controller
    {

        QRCodeService qRCodeService = new QRCodeService();
        CommonService commonService = new CommonService();
        //
        


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getQRCode(PagingParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = qRCodeService.getQRCodeView(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doChange(String UUID,String ISSHOW)
        {
            JObject jo = new JObject();
            try
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string roleid = ticket.UserData;
                string userid = User.Identity.Name;
                
                int i = qRCodeService.doChange(UUID,ISSHOW);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getQRCodeDTL(String UUID)
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            if (StringUtils.getString(UUID) != "")
            {
                int uuid = System.Convert.ToInt32(UUID);
                jo = qRCodeService.getQRCodeDTL(uuid);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            else
            {
                return null;
            }
            
           
        }
        /*
        public ActionResult getCORP()
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            ja = fnNewperService.getCorp();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }*/

     


 

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
                string destFile = "/var/www/html/HW2011/beta/WebBooking/images/qrcode/" + fileName;
                Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.10", "22", "root", "mka89444");
                sftp.Put(srcFile, destFile);
            }
        }


        public void uploadCoupon(HttpPostedFileBase file)
        {
            string fileName = "";
            string srcFile = "";
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
                string destFile = "/var/www/html/HW2011/beta/WebBooking/images/qrcode/coupon/" + fileName;
                Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.10", "22", "root", "mka89444");
                sftp.Put(srcFile, destFile);
            }
        }


        public ActionResult doEditDTL(QRCODE_DTL param)
        {
            JObject jo = new JObject();
            try
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string roleid = ticket.UserData;
                string userid = User.Identity.Name;
                param.EDIT_USER_DTL = userid;

                int i = qRCodeService.doEditDTL(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doEditURL(QRCODE_DTL param)
        {
            JObject jo = new JObject();
            try
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                string roleid = ticket.UserData;
                string userid = User.Identity.Name;
                param.EDIT_USER_DTL = userid;

                int i = qRCodeService.doEditURL(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
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
                
                int i = qRCodeService.doRemove(UUID,userid);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doRemoveDTL(int UUID)
        {
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;


            JObject jo = new JObject();
            try
            {

                int i = qRCodeService.doRemoveDTL(UUID, userid);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public FileResult doPrint(String UUID,int w,int h)
        {

            String URL = qRCodeService.getURL(UUID);
            var writer = new BarcodeWriter  //dll裡面可以看到屬性
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions //設定大小
                {
                    Height = w,
                    Width = h,
                    CharacterSet = "UTF-8"
                }
            };
            Bitmap img = writer.Write(URL); //轉QRcode的文字  


            byte[] byteArray = ImageToByte(img);
            return File(byteArray, "image/png");                      
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        
        public FileResult doRunReport(String SDT,String EDT, String  SUBJECT)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;


            


            byte[] renderedBytes = null;
            string mimeType = null;
            var localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/QRCode/qrcode.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("QRCodeDataSet", qRCodeService.getDataTableByQRCode(SDT,EDT,SUBJECT));
            

            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);


            localReport.Refresh();

            string reportType = "pdf";
            string encoding, fileNameExtension;
            Warning[] warnings;
            string[] streams;

            renderedBytes = localReport.Render(
                reportType,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            String fileName = "report_" + nowStr + ".pdf";

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);

            return File(renderedBytes, mimeType);
        }
    
        
    }
}
