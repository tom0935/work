using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HowardCoupon.Poco;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HowardCoupon_vs2010.Models.Service;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.IO;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Models.Common;
using System.Data;

namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class IsuCheckController : Controller
    {
        //
        // GET: /PayType/
        private IsuCheckService isuCheckService = new IsuCheckService();
        private IsuService isuService = new IsuService();
        private CommonService commonService = new CommonService();
        private CcService ccService = new CcService();
        private CusCompanyService cusCompanyService = new CusCompanyService();
       

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getIsu(CouponIsuPoco param, String UUID,String QueryMode)
        {
           
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            Hashtable ht = commonService.getUserInfo(userid);
            
            String COMP = HashtableUtil.getValue(ht, "COMP_CODE");  //申請公司
            String DEPART = HashtableUtil.getValue(ht, "DEPART_CODE");//申請部門    
            JObject jo = new JObject();
            jo = isuCheckService.getIsu(param, UUID, COMP, DEPART, QueryMode);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult htmlEditor()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult doSave(CouponIsuPoco param, HttpPostedFileBase img)
        {

          //  String uploadPath= System.Web.Configuration.WebConfigurationManager.AppSettings["CouponTemplate"];

            if (img != null)
            {
                

                String fileName = Path.GetFileName(img.FileName);
                String extf=fileName.Substring(fileName.IndexOf("."),4);
                if (param.UUID != null)
                {

                    fileName = param.UUID +extf;
                }
                var path = Path.Combine(Server.MapPath("~/CouponTemplateImage"), fileName);                
                img.SaveAs(path);
            }
		

            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            Hashtable ht= commonService.getUserInfo(userid);
            param.AUSERID = userid;                                  //申請人員代碼            
            param.AENTID = HashtableUtil.getValue(ht, "COMP_CODE");  //申請公司
            param.ADPTID = HashtableUtil.getValue(ht, "DEPART_CODE");//申請部門           

            JObject jo = isuService.doSave(param);
            return Content(JsonConvert.SerializeObject(jo), "text/html");
        }

        public ActionResult doAdd()
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            Hashtable ht = commonService.getUserInfo(userid);

            String AENTNAME = HashtableUtil.getValue(ht, "COMP_NAME");  //申請公司
            String ADPTNAME = HashtableUtil.getValue(ht, "DEPART_NAME");//申請部門  
            String AUSERNAME = HashtableUtil.getValue(ht, "USERNAME");//申請人員  
            String AENTID = HashtableUtil.getValue(ht, "COMP_CODE");  //申請公司
            String ADPTID = HashtableUtil.getValue(ht, "DEPART_CODE");//申請部門  
            String AUSERID = HashtableUtil.getValue(ht, "USERNAME");//申請人員 
            JArray ja = new JArray();
            var itemObject = new JObject
                    {   
                        {"AENTNAME",AENTNAME}, 
                        {"ADPTNAME",ADPTNAME}, 
                        {"AUSERNAME",AUSERNAME},                       
                        {"AUSERID",userid}, 
                        {"AENTID",AENTID}, 
                        {"ADPTID",ADPTID}, 
                    };
            ja.Add(itemObject);
            return Content(JsonConvert.SerializeObject(ja), "text/html");
        }

        /*
        public ActionResult doCopy(String UUID)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            Hashtable ht = commonService.getUserInfo(userid);
            
            String AENTID = HashtableUtil.getValue(ht, "COMP_CODE");  //申請公司
            String ADPTID = HashtableUtil.getValue(ht, "DEPART_CODE");//申請部門  

            int i = 0;
            i = isuCheckService.doCopy(UUID, userid, AENTID, ADPTID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = isuCheckService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }
         

        //取回種類列表 Combobox
        public ActionResult getCCCODE(String CID)
        {
            JArray ja = new JArray();

            ja = ccService.getCcList();
            if (CID == "*")
            {
                var itemObject = new JObject
                    {                           
                        {"NAME","全部"},
                        {"CID",""},
                    };
                ja.AddFirst(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回用途列表 Combobox
        public ActionResult getUSECODE()
        {
            JArray ja = new JArray();
            ja = commonService.getCodeList("COUPON", "USE");
            return Content(JsonConvert.SerializeObject(ja), "application/json");            
        }

        //取回狀態 Combobox
        public ActionResult getSTATUS()
        {
            JArray ja = new JArray();
            var itemObject = new JObject
                    {                           
                        {"NAME","全部"},
                        {"CODE","*"},
                    };
            
            ja = commonService.getCodeList("COUPON", "STATUS");            
            ja.AddFirst(itemObject);

            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult getCusCompany(EasyuiParamPoco param,String SEARCH){
            JObject jo = new JObject();
            jo = cusCompanyService.getCusCompany(param, null, SEARCH);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        //取回企業列表
        public ActionResult getENT(String CID)
        {
            JArray ja = new JArray();
            ja = commonService.getENT();
            if (CID == "*")
            {
                var itemObject = new JObject
                    {                           
                        {"NAME","全部"},
                        {"CID",""},
                    };
                ja.AddFirst(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回付款方式
        public ActionResult getPAYCODE()
        {
            JArray ja = new JArray();
            ja = commonService.getCodeList("COUPON", "PAY");
            return Content(JsonConvert.SerializeObject(ja), "application/json");   
        }

        public ActionResult getDPT()
        {
            JArray ja = new JArray();
            ja = commonService.getDPT();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

       
        public ActionResult chkCode(String CID)
        {
            int i = isuService.chkCode(CID);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }
        
        
       public ActionResult chkFile(String f){
           String fstr="";
           String path = Server.MapPath("~/CouponTemplateImage/");
           if (System.IO.File.Exists(path +f +".jpg"))
           {
               fstr =  f + ".jpg";
           }
           else if (System.IO.File.Exists(path +f +".png"))
           {
               fstr =  f + ".png";
           }
           else if (System.IO.File.Exists(path + f + ".bmp"))
           {
               fstr = f + ".bmp";
           }
           else if (System.IO.File.Exists(path + f + ".gif"))
           {
               fstr = f + ".gif";
           }
           
           return Content(JsonConvert.SerializeObject(fstr), "application/json");
        }

       public ActionResult delFile(String f)
       {
           int i = 0;
           String path = Server.MapPath("~/CouponTemplateImage/");
           if (System.IO.File.Exists(path + f))
           {
               System.IO.File.Delete(path + f);               
               i = 1;
           }

           return Content(JsonConvert.SerializeObject(i), "application/json");
       }


       public ActionResult Report()
       {
           return View();
       }


       public FileContentResult doRunReport(String RPT,String UUID)
       {
           JObject jo = new JObject();
         
           String rdlcStr = null;
           String rptName = null;
           DateTime currentTime = System.DateTime.Now;
           String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
           rdlcStr = "~/Report/"+RPT+".rdlc";
           byte[] result;
           rptName = RPT;
 
 
           byte[] renderedBytes = null;
           string mimeType;
           string encoding;
           string fileNameExtension;
           string[] streams;
           Warning[] warnings;
           using (var renderer = new WebReportRenderer(rdlcStr, rptName))
           
           {
               DataTable dt = null;
               dt = isuCheckService.getIsuView(UUID);
               ReportDataSource reportDataSource = new ReportDataSource("PreviewApplyDataSet", dt);

               ReportParameter uuid = new Microsoft.Reporting.WebForms.ReportParameter("UUID", UUID);
               renderer.ReportInstance.SetParameters(new[] { uuid });
               renderer.ReportInstance.DataSources.Add(reportDataSource);



               renderer.ReportInstance.Refresh();


               renderedBytes = renderer.ReportInstance.Render(
                  "pdf",
                  null,
                  out mimeType,
                  out encoding,
                  out fileNameExtension,
                  out streams,
                  out warnings);
               result = renderer.RenderToBytesPDF();
           }

           jo.Add("status", "1");
           return File(renderedBytes, mimeType);

       }

         */




    }
}
