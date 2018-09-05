using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HowardCoupon.Poco;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HowardCoupon_vs2010.Models.Service;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.IO;
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.Common;
using IntranetSystem.Models.Common;
using System.Data;

namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class IsuQueryController : Controller
    {
        //
        // GET: /PayType/
        private IsuService isuService = new IsuService();
        private CommonService commonService = new CommonService();
        private CcService ccService = new CcService();
        private CusCompanyService cusCompanyService = new CusCompanyService();

        public ActionResult Index()
        {
            return View();
        }

     

    }
}
