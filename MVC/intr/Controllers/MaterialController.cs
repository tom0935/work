using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class MaterialController : Controller
    {
        //Models物件
        Models.MaterialDataContext material = new Models.MaterialDataContext();

        //
        // GET: /Material/

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
            Models.MaterialTypeDataContext mt = new Models.MaterialTypeDataContext();
            ViewData.Add("tabs", mt.LoadMaterialType());
            ViewData.Add("menuId", id);

            //內容
            List<Models.Material> m = material.LoadMaterial(id, pageSize, page);

            //分頁
            int total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_MATERIAL", "TID=" + id));
            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));

            return View(m);
        }

        //
        // GET: /Material/Create

        public ActionResult Create()
        {
            ViewData.Add("TYPE", material.GetTypeList());
            return View();
        }

        //
        // POST: /Material/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.Material m = new Models.Material();
                m.TID = decimal.Parse(collection["TID"]);
                m.CODE = collection["CODE"];
                m.NAME = collection["NAME"];
                m.SPEC = collection["SPEC"];
                m.UNIT = collection["UNIT"];
                m.SAFETY = (collection["SAFETY"] == "")?0:decimal.Parse(collection["SAFETY"]);
                m.REMARK = collection["REMARK"];
                material.CreateMaterial(m);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                ViewData.Add("TYPE", material.GetTypeList());
                return View();
            }
        }

        //
        // GET: /Material/Edit/5

        public ActionResult Edit(int id)
        {
            Models.Material m = material.EditMaterial(id);
            ViewData.Add("TYPE", material.GetTypeList(m.TID));
            return View(m);
        }

        //
        // POST: /Material/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.Material m = new Models.Material();
                m.MID = id;
                m.TID = decimal.Parse(collection["TID"]);
                m.CODE = collection["CODE"];
                m.NAME = collection["NAME"];
                m.SPEC = collection["SPEC"];
                m.UNIT = collection["UNIT"];
                m.SAFETY = decimal.Parse(collection["SAFETY"]);
                m.REMARK = collection["REMARK"];
                material.UpdateMaterial(m);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Models.Material m = material.EditMaterial(id);
                ViewData.Add("TYPE", material.GetTypeList(m.TID));
                ViewData.Add("error", e.Message.ToString());
                return View(m);
            }
        }

        //
        // GET: /Material/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                //取得使用者資料
                FormsIdentity user = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = user.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "Material", aid))
                {
                    material.DeleteMaterial(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }

        }

        //
        // POST: /Material/SearchMaterial

        [HttpPost]
        public PartialViewResult SearchMaterial(int id, string keywords)
        {
            //頁籤選單
            Models.MaterialTypeDataContext mt = new Models.MaterialTypeDataContext();
            ViewData.Add("tabs", mt.LoadMaterialType());
            ViewData.Add("menuId", id);

            List<Models.Material> m = material.SearchMaterial(id, keywords);
            return PartialView(m);
        }

        //
        // GET: /Material/TonerRef/id
        public PartialViewResult TonerRef(int id)
        {
            string refTable = material.GetTonerRef(id);
            ViewData.Add("RefTable", refTable);
            return PartialView();
        }
    }
}
