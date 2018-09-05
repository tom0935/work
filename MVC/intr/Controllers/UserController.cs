using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class UserController : Controller
    {
        //Models物件
        Models.UserDataContext user = new Models.UserDataContext();

        //
        // GET: /User/

        public ActionResult Index(int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //分頁
            page = (page < 1) ? 1 : page;
            int pageSize = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"]);
            int total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_USER", ""));
            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));
            List<Models.User> u = user.LoadUser(pageSize, page);
            return View(u);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                Models.User u = new Models.User();
                u.USERID = collection["USERID"];
                u.PASSWORD = collection["PASSWORD"];
                u.NAME = collection["NAME"];
                u.EMAIL = collection["EMAIL"];
                u.REMARK = collection["REMARK"];
                u.CAKE_COMPANY = collection["CAKE_COMPANY"];
                u.CAKE_DEPARTMENT = collection["CAKE_DEPARTMENT"];
                user.CreateUser(u);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            Models.User u = user.EditUser(id);
            return View(u);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.User u = new Models.User();
                u.AID = id;
                u.USERID = collection["USERID"];
                u.PASSWORD = (string.IsNullOrEmpty(collection["PASSWORD"].ToString())) ? null : collection["PASSWORD"];
                u.NAME = collection["NAME"];
                u.EMAIL = collection["EMAIL"];
                u.REMARK = collection["REMARK"];
                u.CAKE_COMPANY = collection["CAKE_COMPANY"];
                u.CAKE_DEPARTMENT = collection["CAKE_DEPARTMENT"];
                user.UpdateUser(u);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                Models.User u = user.EditUser(id);
                ViewData.Add("error", e.Message.ToString());
                return View(u);
            }
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                //取得使用者資料
                FormsIdentity userident = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = userident.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "User", aid))
                {
                    user.DeleteUser(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /User/Disable/5

        public ActionResult Disable(int id)
        {
            try
            {
                // TODO: Add delete logic here
                //取得使用者資料
                FormsIdentity userident = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = userident.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "User", aid))
                {
                    user.DisableUser(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /User/Enable/5

        public ActionResult Enable(int id)
        {
            try
            {
                // TODO: Add delete logic here
                //取得使用者資料
                FormsIdentity userident = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = userident.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "User", aid))
                {
                    user.EnableUser(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
                return RedirectToAction("Index");
            }
        }


        //
        // GET: /User/ChangePwd/

        public ActionResult ChangePwd()
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            int aid =Convert.ToInt32(ticket.UserData);

            Models.User u = user.EditUser(aid);  
            
            return View(u);
        }

        //
        // POST: /User/ChangePwd/
        
        [HttpPost]
        public ActionResult ChangePwd(string password)
        {
            try
            {
                //取得使用者資料
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                //int aid = ticket.Version;
                int aid = Convert.ToInt32(ticket.UserData);
                // TODO: Add update logic here
                Models.User u = new Models.User();
                u.AID = aid;
                u.PASSWORD = (string.IsNullOrEmpty(password)) ? null : password;
                string msg = user.ChangePassword(u);
                ViewData.Add("message", msg);
                return View(u);
            }
            catch (Exception e)
            {
                //取得使用者資料
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                int aid = ticket.Version;
                ViewData.Add("error", e.Message.ToString());

                Models.User u = user.EditUser(aid);
                return View(u);
            }
        }

        //
        // POST: /User/SearchUser

        [HttpPost]
        public PartialViewResult SearchUser(string keywords)
        {
            List<Models.User> u = user.SearchUser(keywords);
            return PartialView(u);
        }

    }
}
