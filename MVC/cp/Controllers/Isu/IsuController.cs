using System;
using System.Collections.Generic;
using System.Linq;

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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Web;

namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class IsuController : Controller
    {
        //
        // GET: /PayType/
        private IsuService isuService = new IsuService();
        private CommonService commonService = new CommonService();
        private CcService ccService = new CcService();
        private CusCompanyService cusCompanyService = new CusCompanyService();
       

        public ActionResult Index()
        {
            //Byte[] b = CommonService.getPdf();
            //commonService.convPdf();
            return View();
            

        }

        public ActionResult getIsu(CouponIsuPoco param, String UUID,String QueryMode,String SEARCH)
        {
              string[] strArray = new string [] { "1", "2"};
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            Hashtable ht = commonService.getUserInfo(userid);
            
            String COMP = HashtableUtil.getValue(ht, "COMP_CODE");  //申請公司
            String DEPART = HashtableUtil.getValue(ht, "DEPART_CODE");//申請部門    
            JObject jo = new JObject();
            jo = isuService.getIsu(param, UUID, COMP, DEPART, QueryMode,SEARCH);
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

            if(param.STATUS=="3"){    //審核狀態時注入當下審核人員
                param.IUSERID = userid;
            }
           
            JObject jo = isuService.doSave(param);
            
            /*
            if (i > 0)
            {
               int x= isuService.doSaveCI(param);
            }*/
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
            i = isuService.doCopy(UUID, userid, AENTID, ADPTID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = isuService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        //取回種類列表 Combobox
        public ActionResult getCCCODE(String CID)
        {
            JArray ja = new JArray();

            //String[] notinStrArray = new string[] { "44"};
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

        //取回結帳方式
        public ActionResult getREFCODE()
        {
            JArray ja = new JArray();
            ja = commonService.getCodeList("COUPON", "REF");
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult getDPT()
        {
            JArray ja = new JArray();
            ja = commonService.getDPT();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        /*
        public ActionResult chkCode(String CID)
        {
            int i = isuService.chkCode(CID);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }*/

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
           else if (System.IO.File.Exists(path + f ))
           {
               fstr = f ;
           }
           
           return Content(JsonConvert.SerializeObject(fstr), "application/json");
        }


       public ActionResult chkTemplateFile(String f,String CID)
       {
           String fstr = "";
           String path = Server.MapPath("~/Template/" + CID +"/");
           if (System.IO.File.Exists(path + f ))
           {
               fstr = f;
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

        [ValidateInput(false)]
       public FileContentResult OpenPDF(String RPT)
       {
           String templatePath ="" ;
           if (RPT.IndexOf(":") <= 0)
           {
               templatePath = System.Web.HttpContext.Current.Server.MapPath(RPT);
           }
           /*
           var templatePath = System.Web.HttpContext.Current.Server.MapPath(RPT);
           MemoryStream mem = new MemoryStream();
           Document document = new Document();
           PdfWriter writer = PdfWriter.GetInstance(document, mem);        
           document.Open();
           PdfContentByte cb = writer.DirectContent;
           PdfImportedPage newPage;
           PdfReader reader = new PdfReader(templatePath);
           document.NewPage();
           newPage = writer.GetImportedPage(reader, j);
           cb.AddTemplate(newPage, 0, 0);
           w.setPageEmpty(false);
           document.Close();
           //以下為輸出PDF至瀏覽器畫面
           Response.Clear();
           Response.AddHeader("Content-Disposition", "inline; filename=test.pdf");
           Response.ContentType = "Application/PDF";
           Response.OutputStream.Write(mem.GetBuffer(), 0, mem.GetBuffer().Length);
           Response.OutputStream.Flush();
           Response.OutputStream.Close();
           Response.Flush();
           Response.End();*/
           if (RPT.IndexOf(":") > 0)
           {
               return File(System.IO.File.ReadAllBytes(@RPT), "application/pdf");
           }
           else
           {
               return File(System.IO.File.ReadAllBytes(templatePath), "application/pdf");
           }

       }

       public void PreviewPDF(String RPT,String CCID)
       {
          var outPath =System.Web.HttpContext.Current.Server.MapPath("~/Report/out.pdf");
          var templatePath = System.Web.HttpContext.Current.Server.MapPath("~/Template/"+CCID +"/"+RPT);
          var fontPath = System.Web.HttpContext.Current.Server.MapPath("~/Report/fonts/FREE3OF9.TTF");
          MemoryStream mem = new MemoryStream();

            BaseFont baseFont = BaseFont.CreateFont(fontPath,BaseFont.IDENTITY_H,BaseFont.NOT_EMBEDDED);
            Font barcodeFont =new Font(baseFont,20);  //字體大小
            Font numFont = new Font(Font.FontFamily.HELVETICA, 9);
            PdfReader reader;
            var pgSize = new iTextSharp.text.Rectangle(800f, 600f);
            Document document = new Document();
           // PdfWriter writer = PdfWriter.GetInstance(document,new FileStream(outPath,FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, mem);
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            
         //  PdfReader reader1 = new PdfReader(mem.GetBuffer()); //讀取記憶體中的PDF
         for(int i = 0;i < 1;i++)
            {
                reader = new PdfReader(templatePath);
                int iPageNum = reader.NumberOfPages;
                for(int j=1;j<=iPageNum;j++)
                {
                    //document.Open();
                    
                    //document.Close();
                    document.NewPage();
                    document.Add(new Paragraph("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"));
                    Paragraph pg = new Paragraph(10f, "12345678910" + i, barcodeFont);
                    pg.Alignment = Element.ALIGN_RIGHT;
                    Paragraph pg2 = new Paragraph(10f, "12345678910" + i, numFont);
                    pg2.Alignment = Element.ALIGN_RIGHT;
                    document.Add(pg);
                    document.Add(pg2);
                    newPage = writer.GetImportedPage(reader,j);
                    cb.AddTemplate(newPage,0,0);
                }
            }
            document.Close();
            //以下為輸出PDF至瀏覽器畫面
            Response.Clear();
            //Response.AddHeader("Content-Disposition", "attachment;filename=test.pdf");
            //Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "inline; filename=CouponPreview.pdf");
            Response.ContentType = "Application/PDF"; 
            Response.OutputStream.Write(mem.GetBuffer(), 0, mem.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.Flush();
            Response.End();
            
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
               dt = isuService.getIsuView(UUID);
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



  


    }
}
