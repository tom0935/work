using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Web.Http;
using IntranetSystem.Service;
using IntranetSystem.Models;
using IntranetSystem.Poco;
using System.Web.Security;
using Microsoft.Reporting.WebForms;
using IntranetSystem.Models.Service;
using IntranetSystem.Models.Edmx;
using System.IO;

namespace IntranetSystem.FN
{
    public class param
    {
        public string UUID { get; set; }
    }     

    public class FnNewperController : Controller
    {

        FnNewperService fnNewperService = new FnNewperService();
        CommonService commonService = new CommonService();
        //
        


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getNewper(PagingParamPoco param)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            //  ViewBag.Message = "您的應用程式描述頁面。";
            JArray ja = new JArray();
            JObject jo = new JObject();
            string userid = User.Identity.Name;
            jo = fnNewperService.getNewper(param);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getCORP()
        {
            JArray ja = new JArray();
            JObject jo = new JObject();
            ja = fnNewperService.getCorp();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult doCreate(CERTIFICATE_FEE param)
        {
            JObject jo = new JObject();
            try
            {
                int i = fnNewperService.doCreate(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doEdit(CERTIFICATE_FEE param)
        {
            JObject jo = new JObject();
            try
            {
                int i = fnNewperService.doEdit(param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doRemove([System.Web.Http.FromBody] List<CERTIFICATE_FEE> list)
        {
            JObject jo = new JObject();
            try
            {

                int i = fnNewperService.doRemove(list);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", "儲存失敗!系統發生錯誤,請洽資訊人員。錯誤訊息:" + e.TargetSite + "。" + e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public FileResult doPrint(String UUID, String  CORP)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;


            ViewData.Add("DepartmentMsg", Models.NavigationMenu.getDepartmentData(aid));
            String deptStr = null;
            foreach (String deptObj in Models.NavigationMenu.getDepartmentData(aid))
            {
                deptStr = deptObj;
            }


            byte[] renderedBytes = null;
            string mimeType = null;
            var localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/FN/labor.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("NewperDataSet", fnNewperService.getDataTableByNewper(UUID,CORP));
            

            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);
            
            //ReportParameter oddno = new Microsoft.Reporting.WebForms.ReportParameter("ODDNO", ODDNO);
            //ReportParameter dept = new Microsoft.Reporting.WebForms.ReportParameter("DEPT", deptStr);

            //localReport.SetParameters(new ReportParameter[] { oddno, dept });

            localReport.Refresh();

            string reportType = "pdf";
            string encoding, fileNameExtension;
            Warning[] warnings;
            string[] streams;

            renderedBytes = localReport.Render(
                reportType,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            String fileName = "report_" + nowStr + ".pdf";

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);
            // return File(renderedBytes, "application/octet-stream");
            //  return File(renderedBytes, mimeType, fileName);
            return File(renderedBytes, mimeType);
        }
    
        
    }
}
