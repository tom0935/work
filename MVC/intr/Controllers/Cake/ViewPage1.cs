using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Service;
using IntranetSystem.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using IntranetSystem.Poco;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IntranetSystem.Controllers.Cake
{
    partial class ViewPage1 : System.Web.UI.Page
    {
        //
        // GET: /ViewPage1/

        protected void Page_Init(object sender, EventArgs e, int RPT, String SDT, String EDT, String COMP, String SDEPART, String EDEPART, String SPOSNO, String EPOSNO,String QTY_TYPE)
        {
            CakeCodeManagerService cakeCodeManagerService = new CakeCodeManagerService();
            JObject jo = new JObject();

            String rdlcStr = null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            if (RPT == 2)
            {
                rdlcStr = "~/Reports/Cake/orderSaleSum.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (RPT == 3)
            {
                rdlcStr = "~/Reports/Cake/orderSaleList.rdlc";
                rptName = "出貨明細表_" + nowStr;
            }
            else if (RPT == 4)
            {
                rdlcStr = "~/Reports/Cake/orderProductSum.rdlc";
                rptName = "點心房出貨產品匯總表_" + nowStr;
            }

            byte[] result;
            var model = new FileContentAndJson();
            byte[] renderedBytes = null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
            using (var renderer = new WebReportRenderer(rdlcStr, rptName))
            {
                ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(RPT, SDT, EDT, COMP, SDEPART, EDEPART, SPOSNO, EPOSNO, QTY_TYPE));
                ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", SDT);
                ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", EDT);
                ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", COMP);
                ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", SDEPART);
                ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", EDEPART);
                ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", SPOSNO);
                ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", EPOSNO);
                renderer.ReportInstance.SetParameters(new[] { sdt, edt, comp, sdepart, edepart, sposno, eposno });
                renderer.ReportInstance.DataSources.Add(reportDataSource);

                renderer.ReportInstance.Refresh();

                /*
                                renderedBytes = renderer.ReportInstance.Render(
                                   "pdf",
                                   null,
                                   out mimeType,
                                   out encoding,
                                   out fileNameExtension,
                                   out streams,
                                   out warnings);



                                result = renderer.RenderToBytesPDF();*/
                //jo.Add("status", "1");
                
            }              
        }

    }
}
