<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>
<script runat="server">


     protected void Page_Init(object sender, EventArgs e)
    {
     IntranetSystem.Service.CakeCodeManagerService cakeCodeManagerService = new IntranetSystem.Service.CakeCodeManagerService();
            Newtonsoft.Json.Linq.JObject jo = new Newtonsoft.Json.Linq.JObject();

            String rdlcStr = null;
            String rptName = null;
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            var rpt = Request["RPT"];
            if ( int.Parse(rpt) == 2)
            {
                rdlcStr = "~/Reports/Cake/orderSaleSum.rdlc";
                rptName = "出貨彙總表_" + nowStr;
            }
            else if (int.Parse(rpt) == 3)
            {
                rdlcStr = "~/Reports/Cake/orderSaleList.rdlc";
                rptName = "出貨明細表_" + nowStr;
            }
            else if (int.Parse(rpt) == 4)
            {
                rdlcStr = "~/Reports/Cake/orderProductSum.rdlc";
                rptName = "點心房出貨產品匯總表_" + nowStr;
            }

            byte[] result;
            
            byte[] renderedBytes = null;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;
            using (var renderer = new IntranetSystem.Models.Common.WebReportRenderer(rdlcStr, rptName))
            {

                ReportDataSource reportDataSource = new ReportDataSource("OrderDataSet", cakeCodeManagerService.getDataTableByCAKE_ORDERS_VIEW(int.Parse(rpt), Request["SDT"], Request["EDT"], Request["COMP"], Request["SDEPART"], Request["EDEPART"], Request["SPOSNO"], Request["EPOSNO"]));
                ReportParameter sdt = new Microsoft.Reporting.WebForms.ReportParameter("SDT", Request["SDT"]);
                ReportParameter edt = new Microsoft.Reporting.WebForms.ReportParameter("EDT", Request["EDT"]);
                ReportParameter comp = new Microsoft.Reporting.WebForms.ReportParameter("COMP", Request["COMP"]);
                ReportParameter sdepart = new Microsoft.Reporting.WebForms.ReportParameter("SDEPART", Request["SDEPART"]);
                ReportParameter edepart = new Microsoft.Reporting.WebForms.ReportParameter("EDEPART", Request["EDEPART"]);
                ReportParameter sposno = new Microsoft.Reporting.WebForms.ReportParameter("SPOSNO", Request["SPOSNO"]);
                ReportParameter eposno = new Microsoft.Reporting.WebForms.ReportParameter("EPOSNO", Request["EPOSNO"]);
                ReportViewer1.LocalReport.SetParameters(new[] { sdt, edt, comp, sdepart, edepart, sposno, eposno });
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                
                ReportViewer1.LocalReport.Refresh();
            }
    }
</script>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    
    </asp:ScriptManager>
    <div>
    
    </div>
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
        Font-Size="8pt" InteractiveDeviceInfos="(集合)" 
        WaitMessageFont-Names="Verdana" Width="100%" Height="100%" ZoomMode="PageWidth" SizeToReportContent="True"
        WaitMessageFont-Size="14pt"  AsyncRendering="false" 
        ShowParameterPrompts="true" ShowPrintButton="true" DocumentMapWidth="100%" PromptAreaCollapsed="True"
        >
        <LocalReport ReportPath="Reports\Cake\orderSaleSum.rdlc" EnableHyperlinks="true">
         
        </LocalReport>
    </rsweb:ReportViewer>
    </form>
</body>
</html>
