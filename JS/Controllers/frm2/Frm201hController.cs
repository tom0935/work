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
    public class Frm201hController : Controller
    {

        CommonService commonService = new CommonService();
        //
        Frm201hService frm201hService = new Frm201hService();
        CustomerService customerService = new CustomerService();
        EmployeeService employeeService = new EmployeeService();


        public ActionResult frm201h()
        {
            return View();
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


        public ActionResult getDealer(String CNO)
        {
            String tmp = frm201hService.getDealer(CNO);
            return Content(JsonConvert.SerializeObject(tmp), "application/json");
        }

        public ActionResult doSave(Frm201hPoco param)
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
                int i = frm201hService.doSave(param);
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
