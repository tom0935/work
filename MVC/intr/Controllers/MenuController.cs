using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{

    public class MenuController : Controller
    {
        //Models物件
        Models.MenuDataContext menu = new Models.MenuDataContext();

        //
        // GET: /Menu/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Models.Menu> m = menu.LoadMenu();
            return View(m);
        }

        //
        // GET: /Menu/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Menu/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Menu/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.Menu m = new Models.Menu();
                m.NAME = collection["NAME"];
                m.REMARK = collection["REMARK"];
                menu.CreateMenu(m);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

        //
        // GET: /Menu/Edit/5

        public ActionResult Edit(int id)
        {
            Models.Menu m = menu.EditMenu(id);
            return View(m);
        }

        //
        // POST: /Menu/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.Menu m = new Models.Menu();
                m.MID = id;
                m.NAME = collection["NAME"];
                m.REMARK = collection["REMARK"];
                menu.UpdateMenu(m);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Models.Menu m = menu.EditMenu(id);
                ViewData.Add("error", e.Message.ToString());
                return View(m);
            }
        }

        //
        // GET: /Menu/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "Menu", aid))
                {
                    menu.DeleteMenu(id);
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
        // POST: /Menu/UpdateSort/

        [HttpPost]
        public ActionResult UpdateSort(FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                foreach (string key in collection.AllKeys)
                {
                    menu.UpdateMenuSort(key, collection[key]);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                List<Models.Menu> m = menu.LoadMenu();
                ViewData.Add("error", e.Message.ToString());
                return View(m);
            }
        }

    }
}
