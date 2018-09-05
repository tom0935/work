using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class ProgramController : Controller
    {
        //Models物件
        Models.ProgramDataContext program = new Models.ProgramDataContext();

        //
        // GET: /Program/

        public ActionResult Index(int id = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //頁籤選單
            Models.MenuDataContext menu = new Models.MenuDataContext();
            ViewData.Add("tabs", menu.LoadMenu());
            //內容
            List<Models.Program> p = program.LoadProgram(id);
            return View(p);
        }

        //
        // GET: /Program/Create

        public ActionResult Create()
        {
            ViewData.Add("MID", program.getMidList());
            return View();
        }

        //
        // POST: /Program/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.Program p = new Models.Program();
                p.NAME = collection["NAME"];
                p.CODE = collection["CODE"];
                p.MID = decimal.Parse(collection["MID"]);
                p.REMARK = collection["REMARK"];
                p.EXTRA = collection["EXTRA"];
                program.CreateProgram(p);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

        //
        // GET: /Program/Edit/5

        public ActionResult Edit(int id)
        {
            Models.Program p = program.EditProgram(id);
            ViewData.Add("MID", program.getMidList(p.MID));
            return View(p);
        }

        //
        // POST: /Program/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.Program p = new Models.Program();
                p.PID = id;
                p.MID = decimal.Parse(collection["MID"]);
                p.NAME = collection["NAME"];
                p.CODE = collection["CODE"];
                p.REMARK = collection["REMARK"];
                p.EXTRA = collection["EXTRA"];
                program.UpdateProgram(p);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Models.Program p = program.EditProgram(id);
                ViewData.Add("MID", program.getMidList(p.MID));
                ViewData.Add("error", e.Message.ToString());
                return View(p);
            }
        }

        //
        // GET: /Program/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "Program", aid))
                {
                    program.DeleteProgram(id);
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
        // POST: /Program/UpdateSort/

        [HttpPost]
        public ActionResult UpdateSort(FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                foreach (string key in collection.AllKeys)
                {
                    program.UpdateProgramSort(key, collection[key]);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                List<Models.Program> p = program.LoadProgram();
                ViewData.Add("error", e.Message.ToString());
                return View("Index",p);
            }
        }

    }
}
