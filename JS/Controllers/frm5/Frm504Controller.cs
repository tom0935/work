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
    public class Frm504Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm504Service frm504Service = new Frm504Service();



        public ActionResult frm504()
        {
            return View();
        }





      
                public ActionResult doSave(Frm504Poco param)
                {
                    JObject jo = new JObject();
                    try
                    {
                        int i = frm504Service.doSave(param);

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
            var fm = JsonConvert.DeserializeObject<Frm504Poco>(fmJson);
          /*
             List<Frm5042Poco> list=null;
            if (form["list"] != null)
            {
                var listJson = form["list"].ToString();
                list = JsonConvert.DeserializeObject<List<Frm5042Poco>>(listJson);
            }*/
            
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
            rdlcStr = "~/Report/frm504.rdlc";
            byte[] result;
            rptName = "frm504電瓦帳單_" + nowStr;
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

            //    dt = frm504Service.getReport(fm, list);
             
                ReportDataSource reportDataSource = new ReportDataSource("Frm504DataSet", dt);                
                
                ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("SENDDT", fm.SENDDT);
                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("ACP1", fm.ACP1);
                ReportParameter param4 = new Microsoft.Reporting.WebForms.ReportParameter("ACP2", fm.ACP2);
                ReportParameter param5 = new Microsoft.Reporting.WebForms.ReportParameter("SENDER", fm.SENDER);
                ReportParameter param6 = new Microsoft.Reporting.WebForms.ReportParameter("TXNO", fm.TXNO);
                ReportParameter param7 = new Microsoft.Reporting.WebForms.ReportParameter("UPRC", fm.UPRC);
                ReportParameter param8 = new Microsoft.Reporting.WebForms.ReportParameter("PINS", fm.PINS);
                


                renderer.ReportInstance.SetParameters(new[] { param2,param3,param4,param5,param6,param7,param8});
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


            return File(renderedBytes, mimeType, fileName);
        }

        public ActionResult getCONF()
        {
            JArray ja = new JArray();
            ja = frm504Service.getCONF();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

       

    }

}
