using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class MaterialTypeController : Controller
    {
        //Models物件
        Models.MaterialTypeDataContext mtype = new Models.MaterialTypeDataContext();

        //
        // GET: /MaterialType/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Models.MaterialType> mt = mtype.LoadMaterialType();
            return View(mt);
        }

        //
        // GET: /MaterialType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MaterialType/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.MaterialType mt = new Models.MaterialType();
                mt.NAME = collection["NAME"];
                mt.REMARK = collection["REMARK"];
                mtype.CreateMaterialType(mt);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

        //
        // GET: /MaterialType/Edit/5

        public ActionResult Edit(int id)
        {
            Models.MaterialType mt = mtype.EditMaterialType(id);
            return View(mt);
        }

        //
        // POST: /MaterialType/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.MaterialType mt = new Models.MaterialType();
                mt.TID = id;
                mt.NAME = collection["NAME"];
                mt.REMARK = collection["REMARK"];
                mtype.UpdateMaterialType(mt);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Models.MaterialType mt = mtype.EditMaterialType(id);
                ViewData.Add("error", e.Message.ToString());
                return View(mt);
            }
        }

        //
        // GET: /MaterialType/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "MaterialType", aid))
                {
                    mtype.DeleteMaterialType(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View("Index");
            }
        }

    }
}
