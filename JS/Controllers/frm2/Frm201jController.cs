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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;



namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class Frm201jController : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm201jService frm201jService = new Frm201jService();
        CustomerService customerService = new CustomerService();
        EmployeeService employeeService = new EmployeeService();


        public ActionResult frm201j()
        {
            return View();
        }


        public ActionResult getContractDG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm201jService.getContactDG(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }



        //取回異動類別列表 Combobox
        public ActionResult getUPDTP()
        {
            JArray ja = new JArray();
            ja = commonService.getUPDTP();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }


        //取回人員列表 Combobox
        public ActionResult getLOGUSR()
        {
            JArray ja = new JArray();
            ja = employeeService.getLOGUSR();
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }



        public ActionResult doSave(Frm201jPoco param,List<Frm201jPoco> LIST)
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
                int i = frm201jService.doSave(param, LIST);
                
                jo.Add("status", i);
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public ActionResult getYMD(String sdt, String edt)
        {
            JObject jo = new JObject();
            try
            {
                Hashtable ht= frm201jService.getYMD(sdt, edt);
                jo.Add("Y", HashtableUtil.getValue(ht, "y"));
                jo.Add("M", HashtableUtil.getValue(ht, "m"));
                jo.Add("D", HashtableUtil.getValue(ht, "d"));
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

    }        
  
}
