using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IntranetSystem.Poco;
using System.Web.Routing;

namespace IntranetSystem.Controllers
{
    public class WebsiteUploadController : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            requestContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            requestContext.HttpContext.Response.Cache.SetExpires(DateTime.MinValue);
            requestContext.HttpContext.Response.Cache.SetNoStore();
        }

        Models.WebsiteUpload wsul = new Models.WebsiteUpload();

        //
        // GET: /WebsiteUpload/
        
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            List<WebsiteUploadPoco> webAd = wsul.LoadWebsiteAd();
            return View(webAd);
        }

        //
        // GET: /WebsiteUpload/Upload

        public ActionResult Upload()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //
        // POST: /WebsiteUpload/Upload

        [HttpPost]
        public ActionResult Upload(FormCollection collection)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            try
            {
                // TODO: Add insert logic here
                string fileName = string.Empty;
                string srcFile = string.Empty;

                //接收數值
                WebsiteUploadPoco data = new WebsiteUploadPoco();
                data.HOTELSN = int.Parse(collection["HOTELSN"]);
                data.NAME = collection["NAME"];
                data.SDATE = DateTime.Parse(collection["SDATE"] + " " + collection["STIME"] + ":00");
                data.EDATE = DateTime.Parse(collection["EDATE"] + " " + collection["ETIME"] + ":59");
                data.LINK = collection["LINK"];
                data.FILE = string.Empty;
                data.AID = int.Parse(aid);

                //有無上傳檔案
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["FILE"];

                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        fileName = Path.GetFileName(file.FileName);
                        srcFile = Path.Combine(Server.MapPath("~/Uploads/WEB"), fileName);
                        file.SaveAs(srcFile);
                        data.FILE = fileName;
                    }
                }
                //TempData.Add("message", data.FILE);
                wsul.InsertData(data);

                // SFTP上傳檔案
                if (!string.IsNullOrEmpty(data.FILE))
                {
                    string destFile = "/var/www/html/HW2011/beta/images/banners/" + fileName;
                    Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.211", "22", "root", "mka89444");
                    sftp.Put(srcFile, destFile);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
                return View();
            }
        }

        //
        // GET: /WebsiteUpload/Edit/5

        public ActionResult Edit(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            WebsiteUploadPoco data = wsul.GetWebsiteUploadData(id);
            return View(data);
        }

        //
        // POST: /WebsiteUpload/Edit/5

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            try
            {
                // TODO: Add insert logic here
                string fileName = string.Empty;
                string srcFile = string.Empty;

                //接收數值
                WebsiteUploadPoco data = new WebsiteUploadPoco();
                data.UUID = int.Parse(collection["UUID"]);
                data.NAME = collection["NAME"];
                data.SDATE = DateTime.Parse(collection["SDATE"] + " " + collection["STIME"] + ":00");
                data.EDATE = DateTime.Parse(collection["EDATE"] + " " + collection["ETIME"] + ":59");
                data.LINK = collection["LINK"];
                data.FILE = string.Empty;
                data.AID = int.Parse(aid);

                //有無上傳檔案
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["FILE"];

                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        fileName = Path.GetFileName(file.FileName);
                        srcFile = Path.Combine(Server.MapPath("~/Uploads/WEB"), fileName);
                        file.SaveAs(srcFile);
                        data.FILE = fileName;
                    }
                }
                //TempData.Add("message", data.FILE);
                wsul.UpdateData(data);

                // SFTP上傳檔案
                if (!string.IsNullOrEmpty(data.FILE))
                {
                    string destFile = "/var/www/html/HW2011/beta/images/banners/" + fileName;
                    Models.SFTPOperation sftp = new Models.SFTPOperation("10.0.20.211", "22", "root", "mka89444");
                    sftp.Put(srcFile, destFile);
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /WebsiteUpload/Delete/5

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                wsul.Delete(id);
                TempData.Add("message", "資料已刪除。");
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /WebsiteUpload/SortUpdate/

        [HttpPost]
        public ActionResult SortUpdate(string sortData)
        {
            string result = string.Empty;
            try
            {
                wsul.SortUpdate(sortData);
                result = "排序已儲存";
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            return Content(result);
        }

    }
}
