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
using Microsoft.Reporting.Common;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Data;

using System.Linq.Dynamic;
using System.Linq;
using IntranetSystem.Models.Common;
using System.IO;

namespace IntranetSystem.Cake
{


    public class CakeOrdersManagerController : Controller
    {
       CakeOrdersManagerService cakeOrdersManagerService = new CakeOrdersManagerService();
        CommonService commonService = new CommonService();


        //
        // GET: /Cake/


        public ActionResult Index()
        {
            return View();
        }



        public ActionResult OrderGrid(OrderParamPoco param, String STATUS)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = 0;
                Hashtable htUser = commonService.getUserInfo(aid);
                String DEPART_NAME = HashtableUtil.getValue(htUser, "DEPART_NAME");
                param.DEPART = HashtableUtil.getValue(htUser, "DEPART_CODE");

                param.USERID = userid;
                if (param.MODE == "P")
                {
                    ja = cakeOrdersManagerService.getInfoDatagrid(param);
                    i =  cakeOrdersManagerService.getInfoCount(param);
                }
                else
                {
                    i = cakeOrdersManagerService.getOrderCount(param);
                    ja=cakeOrdersManagerService.getOrderDatagrid(param, STATUS);
                }
                
                if (i > 0)
                {
                    jo.Add("total", i);
                    jo.Add("rows", ja);
                }
                else
                {
                    jo.Add("total","");
                    jo.Add("rows", "");
                }

                jo.Add("title", DEPART_NAME);

                //  }else{
                //   jo.Add("total", "0");
                //   jo.Add("rows", ja);
                //  }
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回提醒列表
        public ActionResult InfoGrid(OrderParamPoco param, String STATUS)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = 0;
                Hashtable htUser = commonService.getUserInfo(aid);
                String DEPART_NAME = HashtableUtil.getValue(htUser, "DEPART_NAME");
                param.DEPART = HashtableUtil.getValue(htUser, "DEPART_CODE");

                param.USERID = userid;
                

                i = cakeOrdersManagerService.getInfoCount(param);
                if (i > 0)
                {
                    jo.Add("total", i);
                    jo.Add("rows", cakeOrdersManagerService.getInfoDatagrid(param));
                }
                else
                {
                    jo.Add("total", "");
                    jo.Add("rows", "");
                }

                jo.Add("title", DEPART_NAME);

                //  }else{
                //   jo.Add("total", "0");
                //   jo.Add("rows", ja);
                //  }
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult OrderDetailGrid(OrderParamPoco param)
        {

            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                int i = 0;

                i = cakeOrdersManagerService.getOrderDetailCount(param);

                if (i > 0)
                {

                    //jo.Add("total", i);
                    jo = cakeOrdersManagerService.getOrderDetailDatagrid(param, i);
                    
                }
                else
                {
                    jo.Add("total","");
                    jo.Add("rows", "");
                }


                //  }else{
                //   jo.Add("total", "0");
                //   jo.Add("rows", ja);
                //  }
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getProduct(String POSPNO)
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("rows", cakeOrdersManagerService.getProduct(POSPNO));
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Combobox(String KIND, String CODE)
        {

            JArray jr = new JArray();
            JObject jo = new JObject();
            if (KIND == "1")
            {
                jr = cakeOrdersManagerService.getCombobox1(KIND);
            }
            else
            {
                jr = cakeOrdersManagerService.getCombobox(KIND, CODE);
            }
            return Content(JsonConvert.SerializeObject(jr), "application/json");
        }

        public ActionResult doSave(CAKE_ORDERS odders, CAKE_ORDERS_DTL dtl)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                int i = cakeOrdersManagerService.doSave(htUser, odders, dtl);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



    

        public ActionResult queryOrderDetail(String UUID)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            Decimal uuid = StringUtils.getDecimal(UUID);
            try
            {
                jo.Add("rows", cakeOrdersManagerService.queryOrderDetail(uuid));
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult cancelOrder(String ODDNO,String REMARK)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            int i = 0;
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                i = cakeOrdersManagerService.cancelOrder(htUser, ODDNO,REMARK);                

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult closeOrder(String ODDNO)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            int i = 0;
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                i = cakeOrdersManagerService.closeOrder(htUser, ODDNO);
               // i=cakeOrdersManagerService.closeOrderNew(htUser,List<CAKE_ORDERS_DTL> list);
                  
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult closeOrderNew(List<CAKE_ORDERS_DTL> list)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            int i = 0;
            try
            {
                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
                i=cakeOrdersManagerService.closeOrderNew(htUser,list);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult printOrder(List<CAKE_ORDERS_DTL> list, String STATUS)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            
            int i = 0;
            byte[] renderedBytes = null;
            string mimeType = null;
            try
            {



                Hashtable htUser = commonService.getUserInfo(aid);
                htUser.Add("USERID", userid);
                htUser.Add("AID", aid);
              //  if (list.Count > 0 && STATUS=="O")
              //  {
                   i = cakeOrdersManagerService.printOrder(htUser, list);
              //  }
                /*

                var localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Reports/Cake/orderSingle.rdlc");
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_VIEW(""));

                localReport.DataSources.Clear();
                localReport.DataSources.Add(reportDataSource);
                localReport.Refresh();
                //Setting file
                string reportType = "pdf";
                string encoding, fileNameExtension;

                string deviceInfo =
                "<DeviceInfo><SimplePageHeaders>True</SimplePageHeaders>" +
                    //"  <OutputFormat>+format+</OutputFormat>" +
                    //"  <PageWidth>8.5in</PageWidth>" +
                    //"  <PageHeight>11in</PageHeight>" +
                    //"  <MarginTop>0.5in</MarginTop>" +
                    //"  <MarginLeft>1in</MarginLeft>" +
                    //"  <MarginRight>1in</MarginRight>" +
                    //"  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                
                //Render the report            
                renderedBytes = localReport.Render(
                    reportType,
                    null,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);*/

                jo.Add("status", i);
            }
            catch (Exception e)
            {

            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
         //   return File(renderedBytes, mimeType);
        }

        public FileContentResult printOrderView(String ODDNO)
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            

            ViewData.Add("DepartmentMsg", Models.NavigationMenu.getDepartmentData(aid));
            String deptStr=null;
            foreach(String deptObj in Models.NavigationMenu.getDepartmentData(aid)){                 
                deptStr= deptObj;
            }
            

            byte[] renderedBytes = null;
            string mimeType = null;
            var localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Cake/orderSingle.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("CakeOrder", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_VIEW(ODDNO));
            ReportDataSource reportDataSource1 = new ReportDataSource("CakeOrderDtl", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_DTL_VIEW(ODDNO));

            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(reportDataSource1);
            ReportParameter oddno = new Microsoft.Reporting.WebForms.ReportParameter("ODDNO", ODDNO);
            ReportParameter dept = new Microsoft.Reporting.WebForms.ReportParameter("DEPT", deptStr);
            
            localReport.SetParameters(new ReportParameter[] { oddno,dept});

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
          //  return File(renderedBytes, mimeType);
            DateTime currentTime = System.DateTime.Now;
            String nowStr = String.Format("{0:yyyyMMdd}", currentTime);
            String fileName = "report_" + nowStr + ".pdf";

            String strContentDisposition = String.Format("{0}; filename=\"{1}\"", "attachment", fileName);
             return File(renderedBytes, mimeType);
            //return File(renderedBytes, mimeType, fileName);            
            /*
            
            byte[] result;
            using (var renderer = new WebReportRenderer("~/Reports/Cake/orderSingle.rdlc", "Report.pdf"))
            {

                ReportDataSource reportDataSource = new ReportDataSource("CakeOrder", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_VIEW(ODDNO));
                ReportDataSource reportDataSource1 = new ReportDataSource("CakeOrderDtl", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_DTL_VIEW(ODDNO));
                ReportParameter oddno = new Microsoft.Reporting.WebForms.ReportParameter("ODDNO", ODDNO);
                renderer.ReportInstance.DataSources.Add(reportDataSource);
                renderer.ReportInstance.DataSources.Add(reportDataSource1);
                renderer.ReportInstance.SetParameters(new[] { oddno });
                renderer.ReportInstance.Refresh();
                result = renderer.RenderToBytesPDF();
                
             //   Response.AddHeader("content-disposition", "inline; filename=Report.pdf");
             //   Response.ContentType = "application/pdf";
                //Response.AddHeader("Content-Length", renderedBytes.Length.ToString());
            }
            FileStream fs = new FileStream("output.pdf", FileMode.Create);
                fs.Write(result, 0, result.Length);

            
            //return File("Report.pdf", "application/pdf");
            return File( fs, "text/html");
            */
        }

        //訂購管理列印使用
        public FileContentResult printOrderView2(String ODDNO)
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
            localReport.ReportPath = Server.MapPath("~/Reports/Cake/order.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("CakeOrder", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_VIEW(ODDNO));
            ReportDataSource reportDataSource1 = new ReportDataSource("CakeOrderDtl", cakeOrdersManagerService.getDataTableByCAKE_ORDERS_DTL_VIEW(ODDNO));

            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);
            localReport.DataSources.Add(reportDataSource1);
            ReportParameter oddno = new Microsoft.Reporting.WebForms.ReportParameter("ODDNO", ODDNO);
            ReportParameter dept = new Microsoft.Reporting.WebForms.ReportParameter("DEPT", deptStr);

            localReport.SetParameters(new ReportParameter[] { oddno, dept });

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
         //      Response.AddHeader("content-disposition", "inline; filename=Report.pdf");




  
/*
    Response.Clear();
    Response.Expires = 0;
    Response.CacheControl = "no-cache";

    
    Response.AddHeader("Content-disposition", "attachment; filename=CakeSumReport.pdf");
    Response.ContentType = "text/html";
    Response.AddHeader("Content-Length", renderedBytes.Length.ToString());

    Response.BinaryWrite(renderedBytes);
    Response.Flush();
    Response.End();
            */
        }
   


    }



}
