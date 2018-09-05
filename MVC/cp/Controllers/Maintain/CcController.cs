using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HowardCoupon.Poco;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HowardCoupon_vs2010.Models.Service;
using System.IO;



namespace HowardCoupon_vs2010.Controllers
{
    [Authorize]
    public class CcController : Controller
    {
        //
        // GET: /PayType/
        private CcService ccService = new CcService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getCc(EasyuiParamPoco param,String UUID)
        {
            JObject jo = new JObject();
            jo = ccService.getCc(param,UUID);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        [ValidateInput(false)]
        public ActionResult doSave(CcPoco param, HttpPostedFileBase img)
        {
            if (img != null)
            {
                String fileName = Path.GetFileName(img.FileName);
                String extf = fileName.Substring(fileName.IndexOf("."), 4);
                if (param.CID != null)
                {                    
                    String dt = System.DateTime.Now.ToString("yyyyMMddHH");                    
                    fileName =dt + "_" + param.CID + extf;
                }
                String p = Server.MapPath("~/Template/" + param.CID + "/");

                if (!System.IO.Directory.Exists(p))
                {
                    System.IO.Directory.CreateDirectory(p);
                }
                
                var path = Path.Combine(Server.MapPath("~/Template/"+param.CID +"/"), fileName);

                img.SaveAs(path);
                param.NEW_TEMPLATE = fileName;
            }
            int i = 0;
            i=ccService.doSave(param);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }

        public ActionResult doDelete(String UUID)
        {
            int i = 0;
            i = ccService.doDelete(UUID);
            return Content(JsonConvert.SerializeObject(i), "text/html");
        }


        public ActionResult chkCode(String CID)
        {
            int i = ccService.chkCode(CID);
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

        public ActionResult delTemplateFile(String CID,String f)
        {
            int i = 0;
            String path = Server.MapPath("~/Template/" + CID + "/");

            if (System.IO.File.Exists(path + f))
            {
                System.IO.File.Delete(path + f);
                i = ccService.doSaveTemplate(CID,"");
                i = 1;
            }
            
            return Content(JsonConvert.SerializeObject(i), "application/json");
        }

    }
}
