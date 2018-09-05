using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Service;
using IntranetSystem.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using IntranetSystem.Poco;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;

namespace IntranetSystem.Controllers.Cake
{
    public class CakeCodeManagerController : Controller
    {
        CakeCodeManagerService cakeCodeManagerService = new CakeCodeManagerService();
        private CommonService commonService = new CommonService();
        //
        // GET: /Cake/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getDPT(String CODE)
        {
            JArray ja = new JArray();
            ja = commonService.getDPT(CODE);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public ActionResult LoadingPage()
        {
            JObject jo = new JObject();
           // jo.Add("status", "1");
            return Content("");
        }
        /*
        public ActionResult getAddDetailList(String UUID, String QTY, EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            try
            {
                jo = cakeCodeManagerService.getAddDetailList(UUID, QTY, param);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }*/


        public ActionResult doAddDetail(FixCakeOrderParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;


            int i = 0;
            i = cakeCodeManagerService.doAddDetail(param, userid);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

        public ActionResult getFixCakeOrderList(EasyuiParamPoco param,String UUID,String SEARCH,String SDT,String EDT)
        {
            JObject jo = new JObject();
            jo = cakeCodeManagerService.getFixCakeOrderList(param, UUID, SEARCH,SDT,EDT);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doRemove(String UUID)
        {
            JObject jo = new JObject();
            
            try
            {                
                int i = cakeCodeManagerService.doRemove(UUID);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        /*
        public ActionResult OrderDetailGrid(OrderParamPoco param)
        {

            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = 0;

                i = cakeCodeManagerService.getOrderDetailCount(param);

                if (i > 0)
                {

                    //jo.Add("total", i);
                    jo = cakeCodeManagerService.getOrderDetailDatagrid(param, i);

                }
                else
                {
                    jo.Add("total", "");
                    jo.Add("rows", "");
                }


                //  }else{
                //   jo.Add("total", "0");
                //   jo.Add("rows", ja);
                //  }
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        */

        public FileContentResult doRunReport(int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO, String QTY_TYPE)
        {
            JObject jo = new JObject();
            /*
            byte[] renderedBytes = null;
            string mimeType = null;
            var localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Cake/orderSaleSum.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO));
            
            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);

            ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
            ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
            ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", COMP);
            ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", SDEPART);
            ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", EDEPART);
            ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", SPOSNO);
            ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", EPOSNO);

            localReport.SetParameters(new ReportParameter[] { sdt,edt,comp,sdepart,edepart,sposno,eposno });

            localReport.Refresh();
            //Setting file
            string reportType = "pdf";
            string encoding, fileNameExtension;

            string deviceInfo =
            "<DeviceInfo><SimplePageHeaders>True</SimplePageHeaders>" +
                //"  <OutputFormat>+format+</OutputFormat>" +
                //"  <PageWidth>8.5in</PageWidth>" +
                //"  <PageHeight>11in</PageHeight>" +
                //"  <MarginTop>0.5in</MarginTop>" +
                //"  <MarginLeft>1in</MarginLeft>" +
                //"  <MarginRight>1in</MarginRight>" +
                //"  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;

            //Render the report            
            renderedBytes = localReport.Render(
                reportType,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
          //  return File(renderedBytes, mimeType);
            Response.AddHeader("content-disposition", "attachment; filename=CakeSumReport.pdf");            
            //Response.AddHeader("content-disposition", "inline; filename=AdmissionTicket.pdf");
            //Response.AddHeader("content-disposition", string.Format("inline;filename={0}.pdf", RPT));
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("Content-Length", renderedBytes.Length.ToString());
            return File(renderedBytes, "application/pdf");
            */
            String rdlcStr=null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr= String.Format("{0:yyyyMMdd}",currentTime);

            if (RPT == 6)
            {
                rdlcStr= "~/Reports/Cake/orderSaleSum.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (RPT == 2)
            {
                rdlcStr = "~/Reports/Cake/orderSaleSum.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (RPT == 3)
            {
                rdlcStr = "~/Reports/Cake/orderSaleList.rdlc";
                rptName = "出貨明細表_" + nowStr;
            }
            else if (RPT == 4)
            {
                rdlcStr = "~/Reports/Cake/orderProductSum.rdlc";
                rptName = "點心房出貨產品匯總表_" + nowStr;
            }
            else if (RPT == 5)
            {
                rdlcStr = "~/Reports/Cake/orderDtlFix.rdlc";
                rptName = "點心房出貨調整表_" + nowStr;

            }

            byte[] result;
            var model = new FileContentAndJson();
            byte[] renderedBytes=null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;

           
            using (var renderer = new WebReportRenderer(rdlcStr, rptName))
            {
                DataTable tb = null;
                if (RPT == 5)
                {
                    tb = cakeCodeManagerService.getDataTableByCAKE_ORDERS_DTL_FIX_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO);
                }
                else
                {

                   // tb = cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO, QTY_TYPE);
                    tb = cakeCodeManagerService.getFixView(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO, QTY_TYPE);
                }
                
                ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", tb);
                ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
                ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
                ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", COMP);
                ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", SDEPART);
                ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", EDEPART);
                ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", SPOSNO);
                ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", EPOSNO);
                ReportParameter qty_type = new Microsoft.Reporting.WebForms.ReportParameter("QTY_TYPE", QTY_TYPE);
                renderer.ReportInstance.SetParameters(new[] { sdt, edt, comp, sdepart, edepart, sposno, eposno,qty_type });
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
                //jo.Add("status", "1");
            }
            
            //model.FC = File(result, "application/pdf", rptName);
            //model.CR = Content(JsonConvert.SerializeObject(jo), "application/json");

          //  Response.AddHeader("content-disposition", "inline; filename=CakeSumReport.pdf");
          //  Response.BinaryWrite(result);
          //  Response.End();
            //Response.ContentType = "application/json";
          //  Response.ContentType = "application/json";
            jo.Add("status", "1");
           // return File(result, "application/pdf", rptName);
           // return File(renderedBytes, mimeType);
         //   return Content(JsonConvert.SerializeObject(jo), "application/json");
            DateTime currentTime1 = System.DateTime.Now;
            String nowStr1 = String.Format("{0:yyyyMMdd}", currentTime);
            String fileName = "report_" + nowStr1 + ".pdf";

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);
             return File(renderedBytes, mimeType);
          //  return File(renderedBytes, mimeType, fileName);
        }


        public FileContentResult doRunReportExcel(int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO, String QTY_TYPE)
        {
            JObject jo = new JObject();
            /*
            byte[] renderedBytes = null;
            string mimeType = null;
            var localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Cake/orderSaleSum.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO));
            
            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);

            ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
            ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
            ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", COMP);
            ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", SDEPART);
            ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", EDEPART);
            ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", SPOSNO);
            ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", EPOSNO);

            localReport.SetParameters(new ReportParameter[] { sdt,edt,comp,sdepart,edepart,sposno,eposno });

            localReport.Refresh();
            //Setting file
            string reportType = "pdf";
            string encoding, fileNameExtension;

            string deviceInfo =
            "<DeviceInfo><SimplePageHeaders>True</SimplePageHeaders>" +
                //"  <OutputFormat>+format+</OutputFormat>" +
                //"  <PageWidth>8.5in</PageWidth>" +
                //"  <PageHeight>11in</PageHeight>" +
                //"  <MarginTop>0.5in</MarginTop>" +
                //"  <MarginLeft>1in</MarginLeft>" +
                //"  <MarginRight>1in</MarginRight>" +
                //"  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;

            //Render the report            
            renderedBytes = localReport.Render(
                reportType,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
          //  return File(renderedBytes, mimeType);
            Response.AddHeader("content-disposition", "attachment; filename=CakeSumReport.pdf");            
            //Response.AddHeader("content-disposition", "inline; filename=AdmissionTicket.pdf");
            //Response.AddHeader("content-disposition", string.Format("inline;filename={0}.pdf", RPT));
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("Content-Length", renderedBytes.Length.ToString());
            return File(renderedBytes, "application/pdf");
            */
            String rdlcStr = null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);

            if (RPT == 6)
            {
                rdlcStr = "~/Reports/Cake/orderSaleSumExcel.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (RPT == 2)
            {
                rdlcStr = "~/Reports/Cake/orderSaleSum.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (RPT == 3)
            {
                rdlcStr = "~/Reports/Cake/orderSaleList.rdlc";
                rptName = "出貨明細表_" + nowStr;
            }
            else if (RPT == 4)
            {
                rdlcStr = "~/Reports/Cake/orderProductSum.rdlc";
                rptName = "點心房出貨產品匯總表_" + nowStr;
            }
            else if (RPT == 5)
            {
                rdlcStr = "~/Reports/Cake/orderDtlFix.rdlc";
                rptName = "點心房出貨調整表_" + nowStr;

            }

            byte[] result;
            var model = new FileContentAndJson();
            byte[] renderedBytes = null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
      

            using (var renderer = new WebReportRenderer(rdlcStr, rptName))
            {
                DataTable tb = null;
                if (RPT == 5)
                {
                    tb = cakeCodeManagerService.getDataTableByCAKE_ORDERS_DTL_FIX_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO);
                }
                else
                {

                    // tb = cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO, QTY_TYPE);
                    tb = cakeCodeManagerService.getFixView(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO, QTY_TYPE);
                }

                ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", tb);
                ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
                ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
                ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", COMP);
                ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", SDEPART);
                ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", EDEPART);
                ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", SPOSNO);
                ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", EPOSNO);
                ReportParameter qty_type = new Microsoft.Reporting.WebForms.ReportParameter("QTY_TYPE", QTY_TYPE);
                renderer.ReportInstance.SetParameters(new[] { sdt, edt, comp, sdepart, edepart, sposno, eposno, qty_type });
                renderer.ReportInstance.DataSources.Add(reportDataSource);



                renderer.ReportInstance.Refresh();


                renderedBytes = renderer.ReportInstance.Render(
                   "excel",
                   null,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings);



                result = renderer.RenderToBytesPDF();
                //jo.Add("status", "1");
            }

            //model.FC = File(result, "application/pdf", rptName);
            //model.CR = Content(JsonConvert.SerializeObject(jo), "application/json");

            //  Response.AddHeader("content-disposition", "inline; filename=CakeSumReport.pdf");
            //  Response.BinaryWrite(result);
            //  Response.End();
            //Response.ContentType = "application/json";
            //  Response.ContentType = "application/json";
            jo.Add("status", "1");
            // return File(result, "application/pdf", rptName);
            // return File(renderedBytes, mimeType);
            //   return Content(JsonConvert.SerializeObject(jo), "application/json");
            DateTime currentTime1 = System.DateTime.Now;
            String nowStr1 = String.Format("{0:yyyyMMdd}", currentTime);
            String fileName = "report_" + nowStr1 + ".xls";

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);
            return File(renderedBytes, mimeType);
            //  return File(renderedBytes, mimeType, fileName);
        }



        public ActionResult ViewPage1(int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO,String QTY_TYPE)
        {
            JObject jo = new JObject();
    
            String rdlcStr = null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            if (RPT == 2)
            {
                rdlcStr = "~/Reports/Cake/orderSaleSum.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (RPT == 3)
            {
                rdlcStr = "~/Reports/Cake/orderSaleList.rdlc";
                rptName = "出貨明細表_" + nowStr;
            }
            else if (RPT == 4)
            {
                rdlcStr = "~/Reports/Cake/orderProductSum.rdlc";
                rptName = "點心房出貨產品匯總表_" + nowStr;
            }

            byte[] result;
            var model = new FileContentAndJson();
            byte[] renderedBytes = null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
            using (var renderer = new WebReportRenderer(rdlcStr, rptName))
            {
                ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO,QTY_TYPE));
                ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
                ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
                ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", COMP);
                ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", SDEPART);
                ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", EDEPART);
                ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", SPOSNO);
                ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", EPOSNO);
                //ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", QTY_TYPE);
                renderer.ReportInstance.SetParameters(new[] { sdt, edt, comp, sdepart, edepart, sposno, eposno });
                renderer.ReportInstance.DataSources.Add(reportDataSource);
                
                renderer.ReportInstance.Refresh();
                
/*
                renderedBytes = renderer.ReportInstance.Render(
                   "pdf",
                   null,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings);



                result = renderer.RenderToBytesPDF();*/
                //jo.Add("status", "1");
            }

            //model.FC = File(result, "application/pdf", rptName);
            //model.CR = Content(JsonConvert.SerializeObject(jo), "application/json");

            //  Response.AddHeader("content-disposition", "inline; filename=CakeSumReport.pdf");
            //  Response.BinaryWrite(result);
            //  Response.End();
            //Response.ContentType = "application/json";
            //  Response.ContentType = "application/json";
            jo.Add("status", "1");
            // return File(result, "application/pdf", rptName);
            return View();
            //   return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    }
}
