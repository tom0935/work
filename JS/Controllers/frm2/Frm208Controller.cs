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
using System.Data;
using System.Web.Security;


namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm208Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm208Service frm208Service = new Frm208Service();



        public ActionResult frm208(String YM,String ACCNO,String ACCTP)
        {
            ViewData["YM"] = YM;
            ViewData["ACCNO"] = ACCNO;
            ViewData["ACCTP"] = ACCTP;
            return View();
        }


        public ActionResult getDG1(String MKRNO, String FEEYMS, String FEEYME, String VNO, String ACCTP, String REQUEST_NO, String ACCTP_NO, String ACCNO, String QPAYSTA, String QPAYSTA2)
        {
            JObject jo = new JObject();
            try
            {
                if (StringUtils.getString(ACCTP) != "")
                {
                    jo = frm208Service.getDG1_1(ACCTP);
                }
                else
                {
                    jo = frm208Service.getDG1(MKRNO, FEEYMS, FEEYME, VNO, REQUEST_NO, ACCTP_NO, ACCNO, QPAYSTA, QPAYSTA2);
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

        public ActionResult getDG1ByFrm302(String YM,String ACCNO,String ACCTP)
        {
            JObject jo = new JObject();
            try
            {
                if (StringUtils.getString(ACCTP) != "")
                {
                    jo = frm208Service.getDG1ByFrm302(YM,ACCNO,ACCTP);
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

        


        public ActionResult doSave(Frm208Poco param)
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
                int i = frm208Service.doSave(param);

                    
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doAdd(Frm208Poco param)
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
                int i = frm208Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSuccess(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm208Service.doSuccess(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doRemove(String TAGID, String AMTSUM, String ACCTP, String ACCNO, String FEEYM)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm208Service.doRemove(TAGID,AMTSUM,ACCTP,ACCNO,FEEYM);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //取回付款對象列表 Combobox
        public ActionResult getMAKR(String q)
        {
            JObject jo = new JObject();
            jo = commonService.getMAKR(q);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //費用類別
        public ActionResult getACCTP()
        {
            JArray ja = new JArray();
            ja = commonService.getACCTP();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //費用科目
        public ActionResult getV_ACCF(String CNO)
        {
            JArray ja = new JArray();
            ja = commonService.getV_ACCF(CNO);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //取得稅率
        public ActionResult getTX()
        {           
           String tx = commonService.getConfig("TXRAT");
            return Content(JsonConvert.SerializeObject(tx), "application/json");
        }

        //檢查發票號是否重覆
        public ActionResult doCheckVNO(String FEEYM, String VNO)
        {
            int i = frm208Service.doCheckVNO(FEEYM, VNO);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

        public ActionResult getUSEAMT(String FEEYM, String ACCTP,String ACCNO)
        {
            JArray ja = frm208Service.getUSEAMT(FEEYM, ACCTP,ACCNO);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }



        public ActionResult doExport(String MKRNO, String FEEYM,String VNO)
        {
            var exportSpource = frm208Service.getReport(MKRNO,FEEYM,VNO);
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

            var exportFileName = string.Concat(
                "廠商付款清冊_",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xls");

            return new ExportExcelResult
            {
                SheetName = "廠商付款清冊",
                FileName = exportFileName,
                ExportData = dt
            };


        }
        

    }

}
