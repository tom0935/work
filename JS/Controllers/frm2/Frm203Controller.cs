using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
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


namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm203Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm203Service frm203Service = new Frm203Service();



        public ActionResult frm203()
        {
            return View();
        }


        public ActionResult getDG1(String DT)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm203Service.getDG1(DT);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult getDG3(EasyuiParamPoco param, String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm203Service.getDG3(param, SDT, EDT, FIXAREA, FIXTP, FIXITEM, RMNO, END);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        //取回收入種類列表 Combobox
        public ActionResult getINTP()
        {
            JArray ja = new JArray();
            ja = commonService.getINTP();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回計算結果
        public ActionResult getFormSum(String DT)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm203Service.getFormSum(DT);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult doAdd(Frm203Poco param)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                if (StringUtils.getString(param.LOGUSR) == "")
                {
                    param.LOGUSR = userid;
                }
                int i = frm203Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult doExport(String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
        {
            var exportSpource = frm203Service.getReport(SDT, EDT, FIXAREA, FIXTP, FIXITEM, RMNO, END);
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

            var exportFileName = string.Concat(
                "櫃檯收支作業明細表_",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xlsx");

            return new ExportExcelResult
            {
                SheetName = "櫃檯收支作業明細表",
                FileName = exportFileName,
                ExportData = dt
            };


        }



        public ActionResult doRemove(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm203Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public FileContentResult doRunReportFrm203all(String DT)
        {
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
            rdlcStr = "~/Report/frm203all.rdlc";
            byte[] result;
            rptName = "frm203all";
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
                dt = frm203Service.getReportDetailAll(DT);
                dtSum = frm203Service.getReportSumAll(DT);
                ReportDataSource reportDataSource = new ReportDataSource("Frm203DetailDataSet", dt);
                ReportDataSource reportDataSourceSum = new ReportDataSource("Frm203SumDataSet", dtSum);
                ReportParameter param = new Microsoft.Reporting.WebForms.ReportParameter("DT", DT);
                renderer.ReportInstance.SetParameters(new[] { param });
                renderer.ReportInstance.DataSources.Add(reportDataSource);
                renderer.ReportInstance.DataSources.Add(reportDataSourceSum);
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
            return File(renderedBytes, mimeType, Url.Encode(rptName+".pdf"));
            

        }


        public FileContentResult doRunReportFrm203type(String SDT,String EDT)
        {
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
            rdlcStr = "~/Report/frm203type.rdlc";
            byte[] result;
            rptName = "frm203type";
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
                dt = frm203Service.getReportType(SDT, EDT);                
                ReportDataSource reportDataSource = new ReportDataSource("Frm203TypeDataSet", dt);
                ReportParameter param = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
                ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
                renderer.ReportInstance.SetParameters(new[] { param,param2 });
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
