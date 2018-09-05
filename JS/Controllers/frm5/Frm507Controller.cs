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
    public class Frm507Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm507Service frm507Service = new Frm507Service();



        public ActionResult frm507()
        {
            return View();
        }







      public FileContentResult doRunReport(FormCollection form)
        {
            var fmJson = form["obj"].ToString();            
            var fm = JsonConvert.DeserializeObject<Frm507Poco>(fmJson);
          /*
             List<Frm5072Poco> list=null;
            if (form["list"] != null)
            {
                var listJson = form["list"].ToString();
                list = JsonConvert.DeserializeObject<List<Frm5072Poco>>(listJson);
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
            rdlcStr = "~/Report/frm507.rdlc";
            byte[] result;
            rptName = "frm507年度租金一覽表_" + nowStr;
            byte[] renderedBytes = null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
            int i = 0;
            String fno = "";
            using (var renderer = new WebReportRenderer(rdlcStr, rptName))
            {
                DataTable dt = null;
                DataTable dtSum = null;

                dt = frm507Service.getReport(fm);
             
                ReportDataSource reportDataSource = new ReportDataSource("Frm507DataSet", dt);        
        

                
                ReportParameter param1 = new Microsoft.Reporting.WebForms.ReportParameter("YY", fm.YY);
                
                
//                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("PINS", fm.PINS);



                renderer.ReportInstance.SetParameters(param1);
                renderer.ReportInstance.DataSources.Add(reportDataSource);
               
                renderer.ReportInstance.Refresh();
                renderedBytes = renderer.ReportInstance.Render(
                   fm.FORMAT,
                   null,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings);
                if (fm.FORMAT == "pdf")
                {
                    result = renderer.RenderToBytesPDF();
                    fno = ".pdf";
                }
                else
                {
                    result = renderer.RenderToBytesExcel();
                    fno = ".xls";
                }
            }




         //   jo.Add("status", "1");
            String fileName = rptName + fno;

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);


            return File(renderedBytes, mimeType, fileName);
        }

 
      

    }

}
