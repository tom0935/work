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
    public class Frm302Controller : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm302Service frm302Service = new Frm302Service();



        public ActionResult frm302()
        {
            return View();
        }


        public ActionResult getDG1(String yrn)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm302Service.getDG1(yrn);
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
                jo = frm302Service.getDG2(JOBTAG);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




        public ActionResult doSave(Frm302Poco param, String isSplit)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm302Service.doSave(param, isSplit);

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
                int i = frm302Service.doSaveYM(YMAMT,JOBTAG,TAGID);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult doAdd(Frm302Poco param)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm302Service.doAdd(param);

                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getPrvYRAMT(String yrn,String acctp,String accno)
        {
            int i= frm302Service.getPrvYRAMT(yrn,acctp,accno);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }



        public ActionResult doRemove(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = frm302Service.doRemove(TAGID);

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


        public ActionResult doExport(String YY,String ACCNO,String ACCTP)
        {
            var exportSpource = frm302Service.getReport(YY,ACCNO,ACCTP);
            var dt = JsonConvert.DeserializeObject<DataTable>(exportSpource.ToString());

            var exportFileName = string.Concat(
                "費用預算一覽表_",
                DateTime.Now.ToString("yyyyMMddHHmmss"),
                ".xls");

            return new ExportExcelResult
            {
                SheetName = "費用預算一覽表",
                FileName = exportFileName,
                ExportData = dt
            };


        }

    }

}
