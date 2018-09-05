using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class DepartmentController : Controller
    {
        //Models物件
        Models.DepartmentDataContext depart = new Models.DepartmentDataContext();
        
        //
        // GET: /Department/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Models.Department> department = depart.LoadDepartment();
            return View(department);
        }

        //
        // GET: /Department/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Department/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.Department d = new Models.Department();
                d.NAME = collection["NAME"];
                d.CODE = collection["CODE"];
                d.REMARK = collection["REMARK"];
                depart.CreateDepartment(d);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

        //
        // GET: /Department/Edit/5

        public ActionResult Edit(int id)
        {
            Models.Department department = depart.EditDepartment(id);
            return View(department);
        }

        //
        // POST: /Department/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.Department d = new Models.Department();
                d.DID = id;
                d.CODE = collection["CODE"];
                d.NAME = collection["NAME"];
                d.REMARK = collection["REMARK"];
                depart.UpdateDepartment(d);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Models.Department department = depart.EditDepartment(id);
                ViewData.Add("error", e.Message.ToString());
                return View(department);
            }
        }

        //
        // GET: /Department/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "Department", aid))
                {
                    depart.DeleteDepartment(id);
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
