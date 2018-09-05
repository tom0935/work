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
    public class Frm201eController : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm201eService frm201eService = new Frm201eService();
        CustomerService customerService = new CustomerService();
        EmployeeService employeeService = new EmployeeService();


        public ActionResult frm201e()
        {
            return View();
        }


        public ActionResult getContractDG(String TAGID)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm201eService.getContactDG(TAGID);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        public ActionResult QueryGrid(String TAGID,String FEETP)
        {
            JObject jo = new JObject();
            try
            {
                jo = frm201eService.getQueryDG(TAGID,FEETP);
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



        public ActionResult doSave(Frm201ePoco param,List<Frm201ePoco> LIST)
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
                int i = frm201eService.doSave(param, LIST);
                
                jo.Add("status", i);
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
