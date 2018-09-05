using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm5;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Models.Common;
using System.Data;
using System.IO;



namespace Jasper.Controllers.frm5
{


    [Authorize]
    public class Frm501Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm501Service frm501Service = new Frm501Service();



        public ActionResult frm501()
        {
            return View();
        }

/*
        public ActionResult getDG1(String q,String FEEYM)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm501Service.getDG1(q,FEEYM);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
*/


      
                public ActionResult doSave(Frm501Poco param)
                {
                    JObject jo = new JObject();
                    try
                    {
                        int i = frm501Service.doSave(param);

                        jo.Add("status", i);
                    }
                    catch (Exception e)
                    {
                        jo.Add("status", "0");
                        jo.Add("message", e.StackTrace);
                    }
                    return Content(JsonConvert.SerializeObject(jo), "application/json");
                }




      public FileContentResult doRunReport(FormCollection form)
        {
            var fmJson = form["obj"].ToString();            
            var fm = JsonConvert.DeserializeObject<Frm501Poco>(fmJson);
             List<Frm5012Poco> list=null;
            if (form["list"] != null)
            {
                var listJson = form["list"].ToString();
                list = JsonConvert.DeserializeObject<List<Frm5012Poco>>(listJson);
            }
            MemoryStream workStream = new MemoryStream();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            String rdlcStr = null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            switch (fm.TYPE)
            {
                case "*":
                    rdlcStr = "~/Report/frm501.rdlc";
                    break;
                case "1":
                    rdlcStr = "~/Report/frm5011.rdlc";
                    break;
                case "2":
                    rdlcStr = "~/Report/frm5012.rdlc";
                    break;
                case "3":
                    rdlcStr = "~/Report/frm5013.rdlc";
                    break;
                case "4":
                    rdlcStr = "~/Report/frm5014.rdlc";
                    break;
                case "5":
                    rdlcStr = "~/Report/frm5015.rdlc";
                    break;
                case "6":
                    rdlcStr = "~/Report/frm5016.rdlc";
                    break;
                case "7":
                    rdlcStr = "~/Report/frm5017.rdlc";
                    break;
                case "8":
                    rdlcStr = "~/Report/frm5018.rdlc";
                    break;
            }
            
            byte[] result;
            rptName = "frm501每月帳單_" + nowStr;
            byte[] renderedBytes = null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
            int i = 0;

            using (var renderer = new WebReportRenderer(rdlcStr, rptName))
            {
                DataTable dt = null;
                DataTable dtSum = null;

                dt = frm501Service.getReport(fm, list);
             
                ReportDataSource reportDataSource = new ReportDataSource("Frm501DataSet", dt);                
                ReportParameter param1 = new Microsoft.Reporting.WebForms.ReportParameter("EX1", fm.ETX1);
                ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("EX2", fm.ETX2);
                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("EX3", fm.ETX3);
                ReportParameter param4 = new Microsoft.Reporting.WebForms.ReportParameter("EX4", fm.ETX4);
                ReportParameter param5 = new Microsoft.Reporting.WebForms.ReportParameter("EX5", fm.ETX5);
                ReportParameter param6 = new Microsoft.Reporting.WebForms.ReportParameter("CX1", fm.CTX1);
                ReportParameter param7 = new Microsoft.Reporting.WebForms.ReportParameter("CX2", fm.CTX2);
                ReportParameter param8 = new Microsoft.Reporting.WebForms.ReportParameter("CX3", fm.CTX3);
                ReportParameter param9 = new Microsoft.Reporting.WebForms.ReportParameter("CX4", fm.CTX4);
                ReportParameter param10 = new Microsoft.Reporting.WebForms.ReportParameter("CX5", fm.CTX5);

                renderer.ReportInstance.SetParameters(new[] { param1,param2,param3,param4,param5,param6,param7,param8,param9,param10 });
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

         //   jo.Add("status", "1");
            String fileName = rptName + ".pdf";

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);

          //  Response.ContentType = "application/pdf";
          
          //  Response.setContentLength(baos.toByteArray().length);
          //  Response

          /*
            Response.AddHeader("Accept-Ranges", "bytes");
            Response.AddHeader("Accept-Header", renderedBytes.Length.ToString());
            Response.AddHeader("Cache-Control", "public");
            Response.AddHeader("Cache-Control", "must-revalidate");
            Response.AddHeader("Pragma", "public");

            Response.ContentType = "application/pdf";
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", strContentDisposition);
            Response.AddHeader("Content-Length", renderedBytes.Length.ToString());
            Response.BinaryWrite(renderedBytes);
            Response.Flush();
            Response.End(); */
           // workStream.Write(renderedBytes, 0, renderedBytes.Length);
            return File(renderedBytes, mimeType, fileName);
        }

        public ActionResult getCONF()
        {
            JArray ja = new JArray();
            ja = frm501Service.getCONF();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

       

    }

}
