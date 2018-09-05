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



namespace Jasper.Controllers.frm2
{
    [Authorize]  
    public class Frm201Controller : Controller
    {
        Frm201Service f201service = new Frm201Service();
        Frm107Service f107service = new Frm107Service();
        CommonService commonService = new CommonService();
        //
        ContractService contractService = new ContractService();
        

        public ActionResult frm201(String RMNO)
        {
            if(StringUtils.getString(RMNO)!=""){
                ViewBag.RMNO = RMNO;
            }
            
            return View();
        }

        public ActionResult DealernmDialog()
        {
            return View();
        }

        public ActionResult BrokernmDialog()
        {
            return View();
        }

        public ActionResult CompanyDialog()
        {
            return View();
        }


        public ActionResult ContractDialog()
        {
            return View();
        }

        public ActionResult ContractEditDialog()
        {
            return View();
        }

        public ActionResult ContractManagerDialog()
        {
            return View();
        }

        public ActionResult CustomerDialog()
        {
            return View();
        }

        //續約
        public ActionResult Frm201j()
        {
        
            return View();
        }

        //解約
        public ActionResult Frm201e()
        {
            return View();
        }

        public ActionResult Frm201g()
        {
            return View();
        }

        public ActionResult Frm201f()
        {
            return View();
        }

        public ActionResult Frm201eQueryDialog()
        {
            return View();
        }
        

        public ActionResult getContractDG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = contractService.getContactDG(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getFeeformDG(String TAGID,String FEEYM,String CNNO)
        {
            JObject jo = new JObject();
            try
            {
                String feeym = "";
                /*
                DateTime t = System.DateTime.Now;
                if (DateTimeUtil.getDateTime(FEEYM) > t)
                {
                    feeym = String.Format("{0:yyyyMM}", t);
                }
                else
                {
                    feeym = String.Format("{0:yyyyMM}", DateTimeUtil.getDateTime(FEEYM));
                }*/
                feeym = String.Format("{0:yyyyMM}", DateTimeUtil.getDateTime(FEEYM));

                jo = contractService.getFeeformDG(TAGID, feeym, CNNO);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult getFeeformDtlDG(String TAGID,String FEETP)
        {
            JObject jo = new JObject();
            try
            {
                jo = contractService.getFeeformDtlDG(TAGID,FEETP);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        


        public ActionResult Grid(EasyuiParamPoco param)
        {
            JObject jo = new JObject();
            try
            {
                jo= f201service.getDatagrid(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Grid2(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                jo.Add("rows", f201service.getDatagrid2(CNO));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        public ActionResult Save(String mode, Frm201Poco param)
        {
            JObject jo = new JObject();
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string roleid = ticket.UserData;
            string userid = User.Identity.Name;
            try
            {
                if (StringUtils.getString(param.mLOGUSR) == "")
                {
                    param.mLOGUSR = userid;
                }

                int i = f201service.doSave(mode, param);
                /*
                if (i > 0)
                {
                    f201service.doSaveYRCNT(param.type, StringUtils.getString(param.mHOUSE).Trim(), System.DateTime.Now.Year,StringUtils.getString(param.CNHNO).Trim());
                } */
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult Save2(String mode, MAKRMAN param)
        {
            JObject jo = new JObject();
            
            try
            {
                int i = f201service.doSave2(mode, param);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult doRemoveHouse(String TAGID,String HOUSE)
        {
            JObject jo = new JObject();
            try
            {
                int i = f201service.doRemoveHouse(TAGID,HOUSE);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Remove(String CNO)
        {
            JObject jo = new JObject();
            try
            {
                int i = f201service.doRemove(CNO);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult Remove2(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                int i = f201service.doRemove2(TAGID);
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //房屋牌價
        public ActionResult getPBRENTAL(String CNO)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getPBRENTAL(CNO);
          //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //房屋基本資料
        public ActionResult getROOMF(String CNO)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getROOMF(CNO);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //房屋合約
        public ActionResult getCONTRAH(String CNO,String CNHNO)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getCONTRAH(CNO,CNHNO);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //車位合約
        public ActionResult getCONTRAP(String TAGID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getCONTRAP(TAGID);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //傢俱合約
        public ActionResult getCONTRAF(String TAGID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getCONTRAF(TAGID);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //俱樂部合約
        public ActionResult getCONTRAC(String TAGID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getCONTRAC(TAGID);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        //雜項合約
        public ActionResult getCONTRAA(String TAGID)
        {
            JObject jo = new JObject();
            JArray ja = new JArray();
            ja = f201service.getCONTRAA(TAGID);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        //合約變動記錄
        public ActionResult getUPDNOTE(EasyuiParamPoco param, String TAGID)
        {
            JObject jo = new JObject();            
            jo = f201service.getUPDNOTE(param, TAGID);
            //  jo.Add("result", ja);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //各項租金修改存檔-批次
        public ActionResult doSaveFeeformDG(List<RMFEEM> list)
        {
            JObject jo = new JObject();
            int i = 0;
            try
            {
                 i = contractService.doSaveFeeformDG(list);
                jo.Add("status", i);
                jo.Add("message", "");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //月份金額修改存檔-批次
        public ActionResult doSaveFeeformDtlDG(List<RMFEEM> list)
        {
            JObject jo = new JObject();
            int i = 0;
            try
            {
                 i = contractService.doSaveFeeformDtlDG(list);
                jo.Add("status", i);
                jo.Add("message", "");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getCARLOCT()
        {
            JArray ja = new JArray();
            ja = commonService.getCARLOCT();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
    }
}
