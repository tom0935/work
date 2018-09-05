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
using System.Linq;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Models.Common;
using System.Data;
using System.IO;


namespace Jasper.Controllers.frm5
{
    [Authorize]
    public class Frm503Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm503Service frm503Service = new Frm503Service();



        public ActionResult frm503()
        {
            return View();
        }


        public ActionResult getDG1(Frm503Poco param)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm503Service.getDG1(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(List<Frm503Poco> list)
        {
            JObject jo = new JObject();
  
            try
            {
                if (list != null)
                {                    
                    int i = frm503Service.doSave(list);
                    jo.Add("status", i);
                }
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
            var fm = JsonConvert.DeserializeObject<Frm503Poco>(fmJson);
            List<Frm503Poco> list = null;
            if (form["list"] != null)
            {
                var listJson = form["list"].ToString();
                list = JsonConvert.DeserializeObject<List<Frm503Poco>>(listJson);
            }            
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
            rdlcStr = "~/Report/frm503.rdlc";
            byte[] result;
            rptName = "frm503廠商付款清冊_" + nowStr;
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

                dt = frm503Service.getReport(list);

                ReportDataSource reportDataSource = new ReportDataSource("Frm503DataSet", dt);
                String fixarea="";
                String fixquip = "";
                String paysta = "";
                if(fm.FIXAREA.Trim()=="1"){
                    fixarea = "住宅管理";
                }else if(fm.FIXAREA.Trim()=="2"){
                    fixarea = "俱樂部管理";
                }else if(fm.FIXAREA.Trim()=="3"){
                    fixarea = "公共區管理";
                }else{
                    fixarea = "住宅/俱樂部/公共區";
                }

                if (fm.FIXQUIP.Trim() == "1")
                {
                    fixquip = "設備";
                }
                else if (fm.FIXQUIP.Trim() == "2")
                {
                    fixquip = "其他";
                }
                else
                {
                    fixquip = "設備/其他";
                }

                if (fm.PAYSTA.Trim() == "1")
                {
                    paysta = "已付款給廠商";
                }
                else if (fm.PAYSTA.Trim() == "2")
                {
                    paysta = "未付款給廠商";
                }
                else
                {
                    paysta = "已付款/未付款";
                }

                ReportParameter param1 = new Microsoft.Reporting.WebForms.ReportParameter("FIXAREA", fixarea);
                ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("FIXQUIP", fixquip);
                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("PAYSTA", paysta);
                ReportParameter param4 = new Microsoft.Reporting.WebForms.ReportParameter("FEEYM", StringUtils.getString(fm.FEEYM));

                if(StringUtils.getString(fm.LOGUSR)==""){
                    fm.LOGUSR =userid;
                }
                ReportParameter param5 = new Microsoft.Reporting.WebForms.ReportParameter("LOGUSR", fm.LOGUSR);

                renderer.ReportInstance.SetParameters(new[] { param1, param2, param3,param4,param5});
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





    }

}
