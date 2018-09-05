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
    public class IsuContentEditController : Controller
    {
        //
        // GET: /PayType/
        private IsuContentEditService isuContentEditService = new IsuContentEditService();
     
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult doSaveContent(String UUID, String CONTENT1, String CONTENT2, String CONTENT3, String CONTENT4, String CONTENT5, String CONTENT6)
        {
            int i = 0;
            i = isuContentEditService.doSave(UUID, CONTENT1, CONTENT2, CONTENT3, CONTENT4, CONTENT5, CONTENT6);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }          

    }
}
