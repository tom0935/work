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
    public class Frm521Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm521Service frm521Service = new Frm521Service();



        public ActionResult frm521()
        {
            return View();
        }



        public ActionResult Grid(Frm301Poco param)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                //jo = frm301Service.getDG1(param);
                var yy = (System.DateTime.Now.Year) + 1;
                int y = 0;
                for (int i = 2000; i < yy; i++)
                {
                    var itemObject = new JObject
                    {                                           
                        {"YY",i}
                    };
                    ja.Add(itemObject);
                    y++;
                }

                if (y > 0)
                {
                    jo.Add("rows", ja);
                    jo.Add("total", y);
                }
                else
                {
                    jo.Add("rows", "");
                    jo.Add("total", "");
                }

                    jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }






      public FileContentResult doRunReport(FormCollection form)
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
            rdlcStr = "~/Report/frm521.rdlc";
            byte[] result;
            rptName = "frm521住戶車位一覽表_" + nowStr;
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

                dt = frm521Service.getReport();
             
                ReportDataSource reportDataSource = new ReportDataSource("Frm521DataSet", dt);        
        

                
                //ReportParameter param2 = new Microsoft.Reporting.WebForms.ReportParameter("YY", fm.YY);
//                ReportParameter param3 = new Microsoft.Reporting.WebForms.ReportParameter("PINS", fm.PINS);
                


              //  renderer.ReportInstance.SetParameters( param2);
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
