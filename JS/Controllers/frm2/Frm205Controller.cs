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
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Models.Common;
using System.Data;
using System.Web.Security;


namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm205Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm205Service frm205Service = new Frm205Service();



        public ActionResult frm205()
        {
            return View();
        }


        public ActionResult getDG1(EasyuiParamPoco param, String DT)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm205Service.getDG1(param,DT);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getDG2(EasyuiParamPoco param,String DT)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm205Service.getDG2(param,DT);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getDG3(EasyuiParamPoco param, String SDT,String EDT,String FIXAREA,String FIXTP,String FIXITEM,String RMNO,String END)
        {
        
            JObject jo = new JObject();
            try
            {
                jo = frm205Service.getDG3(param, SDT, EDT, FIXAREA, FIXTP, FIXITEM, RMNO, END);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult doSave(Frm205Poco param)
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
                int i = frm205Service.doSave(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm205Poco param)
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
                int i = frm205Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doSuccess(String TAGID,String ENDNOTE)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm205Service.doSuccess(TAGID,ENDNOTE);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doRemove(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm205Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回修繕別列表 Combobox
        public ActionResult getFIX205(String CODE)
        {
            JArray ja = new JArray();
            ja = commonService.getFIX205(CODE);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取回修繕項目列表 Combobox
        public ActionResult getFIX205ITEM(String CODE)
        {
            JArray ja = new JArray();
            ja = commonService.getFIX205ITEM(CODE);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public void doExport2(String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
        {
            /*

            string userid = User.Identity.Name;
            JObject jo = new JObject();
            String rdlcStr = null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            rdlcStr = "~/Report/frm205.rdlc";
            byte[] result;
            rptName = "frm205";
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
                
                dt = frm205Service.getReport(SDT,  EDT, FIXAREA,  FIXTP,  FIXITEM,  RMNO, END);
                ReportDataSource reportDataSource = new ReportDataSource("f205DataSet", dt);
                DateTime sdt = DateTimeUtil.getDateTime(SDT + " 00:00:00");
                DateTime edt = DateTimeUtil.getDateTime(EDT + " 23:59:59");
                ReportParameter p1 = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
                ReportParameter p2 = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
                ReportParameter p3 = new Microsoft.Reporting.WebForms.ReportParameter("FIXAREA", FIXAREA);
                ReportParameter p4 = new Microsoft.Reporting.WebForms.ReportParameter("FIXTP", FIXTP);
                ReportParameter p5 = new Microsoft.Reporting.WebForms.ReportParameter("FIXITEM", FIXITEM);
                ReportParameter p6 = new Microsoft.Reporting.WebForms.ReportParameter("RMNO", RMNO);
                ReportParameter p7 = new Microsoft.Reporting.WebForms.ReportParameter("END", END);

                renderer.ReportInstance.SetParameters(new[] { p1,p2,p3,p4,p5,p6,p7 });
                renderer.ReportInstance.DataSources.Add(reportDataSource);                
                renderer.ReportInstance.Refresh();
                renderedBytes = renderer.ReportInstance.Render(
                   "Excel",
                   null,
                   out mimeType,
                   out encoding,
                   out fileNameExtension,
                   out streams,
                   out warnings);
                result = renderer.RenderToBytesExcel();
                
                Response.Clear(); 
                Response.Buffer = true; 
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment;filename=report.xls"); Response.BinaryWrite(renderedBytes); Response.Flush();

                Response.Clear();
                Response.Buffer = true;
                Response.Clear();

                Response.AddHeader("Content-Disposition", "attachment; filename=abc.xls");
                Response.AddHeader("Content-Length", renderedBytes.Length.ToString());
                Response.BinaryWrite(renderedBytes);
                Response.End();
 
            }


          //  byte[] fileContents = Encoding.Default.GetBytes(renderedBytes.ToString());
            return File(renderedBytes, "application/ms-excel", "fileContents.xls"); 
            */
          //  jo.Add("status", "1");
          //  return File(renderedBytes, mimeType);

        }

        public ActionResult doExport(String SDT, String EDT, String FIXAREA, String FIXTP, String FIXITEM, String RMNO, String END)
        {
            var exportSpource = frm205Service.getReport(SDT, EDT, FIXAREA, FIXTP, FIXITEM, RMNO, END);
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());
            
            var exportFileName = string.Concat(
                "工程修繕清冊_",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xlsx");

            return new ExportExcelResult
            {
                SheetName = "工程修繕清冊",
                FileName = exportFileName,
                ExportData = dt
            };


        }




    }

}
