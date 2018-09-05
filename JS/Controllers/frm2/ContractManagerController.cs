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



namespace Jasper.Controllers.frm2
{
    [Authorize]
    public class ContractManagerController : Controller
    {

        CommonService commonService = new CommonService();
        //
        ContractManagerService contractManagerService = new ContractManagerService();
        CustomerService customerService = new CustomerService();
        EmployeeService employeeService = new EmployeeService();


        public ActionResult frm201j()
        {
            return View();
        }


        public ActionResult getCmDG1(String TAGID,String CNNO,String FEETP)
        {
            JObject jo = new JObject();
            try
            {
                jo = contractManagerService.getCmDG1(TAGID,CNNO,FEETP);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.ToString());
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }







        public ActionResult doSum(ContractManagerPoco param, List<ContractManagerListPoco> list)
        {
            JObject jo = new JObject();
            try
            {
                int i = contractManagerService.doSum(param, list);
               // int i = 0;
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
