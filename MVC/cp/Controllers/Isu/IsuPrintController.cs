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
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class IsuPrintController : Controller
    {
        //
        // GET: /PayType/
        private IsuPrintService isuPrintService = new IsuPrintService();
        private IsuService isuService = new IsuService();
        private CommonService commonService = new CommonService();
        private CcService ccService = new CcService();
        private CusCompanyService cusCompanyService = new CusCompanyService();
       

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getIsu(EasyuiParamPoco param, String UUID)
        {
           
            JObject jo = new JObject();            
            jo = isuPrintService.getIsu(param,"3",UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public void PrintPDF(String RPT,String ISUID,String CCID)
        {
           // Decimal isuid = StringUtils.getDecimal(UUID);
            var outPath = System.Web.HttpContext.Current.Server.MapPath("~/Report/out.pdf");
            var templatePath = System.Web.HttpContext.Current.Server.MapPath("~/Template/"+CCID+"/" + RPT);
            var fontPath = System.Web.HttpContext.Current.Server.MapPath("~/Report/fonts/FREE3OF9.TTF");
            MemoryStream mem = new MemoryStream();

            List<Hashtable> list = isuPrintService.getIsuCiView(ISUID);

            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font barcodeFont = new Font(baseFont, 20);  //字體大小
            Font numFont = new Font(Font.FontFamily.HELVETICA, 9);
            Font smallFont = new Font(Font.FontFamily.HELVETICA, 7);
            PdfReader reader;
            Document document = new Document();
           //  PdfWriter writer = PdfWriter.GetInstance(document,new FileStream(outPath,FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, mem);
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            if (StringUtils.getString(ISUID) != "")
            {
                isuPrintService.doSave(ISUID, RPT);
            }
            //  PdfReader reader1 = new PdfReader(mem.GetBuffer()); //讀取記憶體中的PDF
            foreach(Hashtable ht in list)
            {
                reader = new PdfReader(templatePath);
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    //document.Open();

                    //document.Close();
                    document.NewPage();
                    document.Add(new Paragraph("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n"));
                    Paragraph pg = new Paragraph(10f, HashtableUtil.getValue(ht,"BARCODE") , barcodeFont);
                    pg.Alignment = Element.ALIGN_RIGHT;
                    Paragraph pg2 = new Paragraph(10f, HashtableUtil.getValue(ht, "BARCODE"), numFont);
                    pg2.Alignment = Element.ALIGN_RIGHT;
                    Paragraph pg3 = new Paragraph(10f, "REF[" + HashtableUtil.getValue(ht, "REF_CODE") + "]/REF[" + HashtableUtil.getValue(ht, "REF2_CODE") + "]/" + HashtableUtil.getValue(ht, "INVOICE") + "/" + HashtableUtil.getValue(ht, "UPRC_CODE") + "/" + HashtableUtil.getValue(ht, "ADPTID"), smallFont);
                    pg3.Alignment = Element.ALIGN_RIGHT;

                    document.Add(pg);
                    document.Add(pg2);
                    document.Add(pg3);
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
            //以下為輸出PDF至瀏覽器畫面
            Response.Clear();
            //Response.AddHeader("Content-Disposition", "attachment;filename=test.pdf");
            //Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "inline; filename=Coupon.pdf");
            Response.ContentType = "Application/PDF";
            Response.OutputStream.Write(mem.GetBuffer(), 0, mem.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.Flush();
            Response.End();

        }
   

      public FileContentResult doRunReport(String RPT,String ISUID)
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
            int i = 0;
           if (StringUtils.getString(ISUID) != "")
           {
               i = isuPrintService.doSave(ISUID,null);  
           }
           using (var renderer = new WebReportRenderer(rdlcStr, rptName))           
           {
               DataTable dt = null;
               dt = isuService.getIsuCiView(ISUID);
               ReportDataSource reportDataSource = new ReportDataSource("PrintCouponDataSet", dt);
               ReportParameter isuid = new Microsoft.Reporting.WebForms.ReportParameter("ISUID", ISUID);
               renderer.ReportInstance.SetParameters(new[] { isuid });
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
