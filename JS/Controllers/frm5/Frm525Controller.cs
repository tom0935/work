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
    public class Frm525Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm525Service frm525Service = new Frm525Service();



        public ActionResult frm525()
        {
            return View();
        }










      public FileContentResult doRunReport(FormCollection form)
        {
            var fmJson = form["obj"].ToString();            
            var fm = JsonConvert.DeserializeObject<Frm525Poco>(fmJson);
          /*
             List<Frm5252Poco> list=null;
            if (form["list"] != null)
            {
                var listJson = form["list"].ToString();
                list = JsonConvert.DeserializeObject<List<Frm5252Poco>>(listJson);
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
            rdlcStr = "~/Report/frm525.rdlc";
            byte[] result;
            rptName = "frm525水電瓦帳單明細表_" + nowStr;
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

                dt = frm525Service.getReport(fm);
             
                ReportDataSource reportDataSource = new ReportDataSource("Frm525DataSet", dt);        
        

                
                ReportParameter param1 = new Microsoft.Reporting.WebForms.ReportParameter("SYM", fm.SYM);
                ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("EYM", fm.EYM);
                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("TOT", fm.TOT);
//                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("PINS", fm.PINS);



                renderer.ReportInstance.SetParameters(new[]{ param1,param2,param3});
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



       

    }

}
