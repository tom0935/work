using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IntranetSystem.Poco;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IntranetSystem.Models.Bulletin;

namespace IntranetSystem.Controllers
{
    public class BulletinController : Controller
    {
        //Models
        Models.Bulletin.Bulletin blt = new Models.Bulletin.Bulletin();

        //
        // GET: /Bulletin/

        public ActionResult Index(int id = 1, int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //檢查參數
            id = (id < 1) ? 1 : id;
            page = (page < 1) ? 1 : page;
            int pageSize = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"]);

            //頁籤選單
            ViewData.Add("tabs", blt.LoadBulletinType());
            ViewData.Add("menuId", id);

            List<BulletinPoco> bulletin = blt.LoadBulletin(id);
            return View(bulletin);
        }

        //
        // GET: /Bulletin/Upload

        public ActionResult Upload()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData.Add("BTYPE", blt.LoadBulletinType());
            return View();
        }


        public ActionResult getGrid(int limit,int offset)
        {

            JObject jo = new JObject();
            JArray ja = new JArray();
            try
            {
                jo = blt.getGrid(1,limit,offset);
            }
            catch (Exception e)
            {

            }

            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        //
        // POST: /Bulletin/Upload

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
                BulletinPoco data = new BulletinPoco();
                data.NAME = collection["NAME"];
                data.BDATE = DateTime.Now;
                data.REMARK = collection["REMARK"];
                data.AID = int.Parse(aid);
                data.BTYPE = int.Parse(collection["BTYPE"]);
                BulletinTypePoco bulletinType = blt.GetBulletinType(int.Parse(collection["BTYPE"]));
                data.CATEGORY = bulletinType.CODE;
                //有無上傳資料
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["FILENAME"];

                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        data.PATH = fileName;
                        string path = Path.Combine(Server.MapPath("~/Uploads/" + data.CATEGORY), fileName);
                        file.SaveAs(path);
                    }

                }
                Models.Bulletin.Bulletin bt = new Models.Bulletin.Bulletin();
                bt.InsertData(data);
            }
            catch (Exception ex)
            {
                TempData.Add("error", ex.Message);
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Bulletin/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Bulletin/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Bulletin/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                //取得使用者資料
                FormsIdentity user = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = user.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "Bulletin", aid))
                {
                    blt.DeleteData(id);
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
            }
            return RedirectToAction("Index");
        }
    }
}
