﻿using Jasper.Models;
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
    public class Frm519Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm519Service frm519Service = new Frm519Service();



        public ActionResult frm519()
        {
            return View();
        }










      public FileContentResult doRunReport(FormCollection form)
        {
            var fmJson = form["obj"].ToString();            
            var fm = JsonConvert.DeserializeObject<Frm519Poco>(fmJson);
          /*
             List<Frm5192Poco> list=null;
            if (form["list"] != null)
            {
                var listJson = form["list"].ToString();
                list = JsonConvert.DeserializeObject<List<Frm5192Poco>>(listJson);
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
            rdlcStr = "~/Report/frm519.rdlc";
            byte[] result;
            rptName = "frm519專線管理報表_" + nowStr;
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

                dt = frm519Service.getReport(fm);
             
                ReportDataSource reportDataSource = new ReportDataSource("Frm519DataSet", dt);        
        

                
                ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("OPT", fm.OPT);
//                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("PINS", fm.PINS);
                


                renderer.ReportInstance.SetParameters( param2);
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
