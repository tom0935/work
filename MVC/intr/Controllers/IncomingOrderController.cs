using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class IncomingOrderController : Controller
    {
        //Models物件
        Models.IncomingOrderDataContext order = new Models.IncomingOrderDataContext();

        //
        // GET: /IncomingOrder/

        public ActionResult Index(int page=1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //分頁
            page = (page < 1) ? 1 : page;
            int pageSize = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"]);
            int total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_INCOMINGORDER"));
            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));
            List<Models.IncomingOrder> o = order.LoadOrder(pageSize, page);
            return View(o);
        }


        //
        // GET: /IncomingOrder/View/5

        public ActionResult View(int id)
        {
            Models.IncomingOrder o = order.EditOrder(id);
            ViewData.Add("DetailView", order.DetailView(id));
            return View(o);
        }


        //
        // GET: /IncomingOrder/Create

        public ActionResult Create()
        {
            //取得使用者資料
            FormsIdentity id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "IncomingOrder", aid))
            {
                Models.IncomingOrder o = order.CreateOrder(aid);
                ViewData.Add("TypelLists", order.TypelLists());
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /IncomingOrder/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                foreach (var item in collection)
                {
                    if (item.ToString() == "REMARK" || item.ToString() == "remark")
                    {
                        order.UpdateOrder(collection["IID"].ToString(), collection["REMARK"].ToString(), collection["STATUS"].ToString());
                    }
                    else if (item.ToString() != "IID" && item.ToString() != "iid" && item.ToString() != "STATUS" && item.ToString() != "status")
                    {
                        order.InsertOrderDetails(collection["IID"].ToString(), item.ToString(), collection[item.ToString()]);
                    }
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /IncomingOrder/Edit/5

        public ActionResult Edit(int id)
        {
            Models.IncomingOrder o = order.EditOrder(id);
            ViewData.Add("TypelLists", order.TypelLists());
            ViewData.Add("Details", order.Details(id));
            return View(o);
        }

        //
        // POST: /IncomingOrder/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                //取得使用者資料
                FormsIdentity user = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = user.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "IncomingOrder", aid))
                {
                    order.DeleteIncomingDetails(id);
                }                

                foreach (var item in collection)
                {
                    if (item.ToString() == "REMARK" || item.ToString() == "remark")
                    {
                        order.UpdateOrder(id.ToString(), collection["REMARK"].ToString(), collection["STATUS"].ToString());
                    }
                    else if (item.ToString() != "IID" && item.ToString() != "iid" && item.ToString() != "STATUS" && item.ToString() != "status")
                    {
                        order.InsertOrderDetails(collection["IID"].ToString(), item.ToString(), collection[item.ToString()]);
                    }
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /IncomingOrder/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "IncomingOrder", aid))
                {
                    order.DeleteIncomingOrder(id);
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
                return View("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
