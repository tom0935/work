using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class GroupController : Controller
    {
        //Models物件
        Models.GroupDataContext group = new Models.GroupDataContext();

        //
        // GET: /Group/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Models.Group> g = group.LoadGroup();
            return View(g);
        }

        //
        // GET: /Group/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Group/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.Group g = new Models.Group();
                g.NAME = collection["NAME"];
                g.CODE = collection["CODE"];
                g.REMARK = collection["REMARK"];
                group.CreateGroup(g);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id)
        {
            Models.Group g = group.EditGroup(id);
            return View(g);
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.Group g = new Models.Group();
                g.GID = id;
                g.NAME = collection["NAME"];
                g.CODE = collection["CODE"];
                g.REMARK = collection["REMARK"];
                group.UpdateGroup(g);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Models.Group g = group.EditGroup(id);
                ViewData.Add("error", e.Message.ToString());
                return View(g);
            }
        }

        //
        // GET: /Group/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "Group", aid))
                {
                    group.DeleteGroup(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

    }
}
