using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.Models.Service;
using Jasper.service;
using Jasper.service.frm1;
using Jasper.service.frm2;
using Jasper.service.frm3;
using Jasper.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;


namespace Jasper.Controllers.frm3
{
    [Authorize]
    public class Frm303Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm303Service frm303Service = new Frm303Service();



        public ActionResult frm303()
        {
            return View();
        }


        public ActionResult getDG1(String yrn)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm303Service.getDG1(yrn);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getDG2(String JOBTAG)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm303Service.getDG2(JOBTAG);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm303Poco param, String isSplit)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm303Service.doSave(param, isSplit);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doSaveYM(String YMAMT, String JOBTAG, String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm303Service.doSaveYM(YMAMT, JOBTAG, TAGID);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doAdd(Frm303Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm303Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getPrvYRAMT(String yrn, String acctp, String accno)
        {
            int i = frm303Service.getPrvYRAMT(yrn, acctp, accno);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }



        public ActionResult doRemove(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm303Service.doRemove(TAGID);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult getINFTP()
        {
            JArray ja = new JArray();
            ja = commonService.getINFTP();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        public ActionResult doExport(String YY, String ACCNO, String ACCTP)
        {
            var exportSpource = frm303Service.getReport(YY, ACCNO, ACCTP);
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

            var exportFileName = string.Concat(
                "收入預估作業一覽表_",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xls");

            return new ExportExcelResult
            {
                SheetName = "收入預估作業一覽表",
                FileName = exportFileName,
                ExportData = dt
            };


        }

    }

}
