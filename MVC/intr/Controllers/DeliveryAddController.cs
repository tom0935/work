using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class DeliveryAddController : Controller
    {
        //Models物件
        Models.DeliveryAddDataContext order = new Models.DeliveryAddDataContext();


        public ActionResult Index(int id = 2, int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            Models.DeliveryOrderDataContext rOrder = new Models.DeliveryOrderDataContext();

            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            //分頁
            page = (page < 1) ? 1 : page;
            int pageSize = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"]);
            int total = 0;
            switch (id)
            {
                default:
                case 2: //已領
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=4"));
                    break;
                case 4: //作廢
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=0"));
                    break;
            }

            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));
            List<Models.DeliveryOrder> o = rOrder.LoadOrder(id, aid, pageSize, page);
            return View(o);
        }

        //
        // GET: /DeliveryAdd/Details/5

        public ActionResult Details(int id)
        {
            Models.RequisitionOrderDataContext rOrder = new Models.RequisitionOrderDataContext();

            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "DeliveryAdd", aid))
            {
                Models.RequisitionOrder o = rOrder.EditOrder(aid, id);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        //
        // GET: /DeliveryAdd/Create

        public ActionResult Create()
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "DeliveryAdd", aid))
            {
                Models.DeliveryAdd o = order.CreateOrder(aid);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /DeliveryAdd/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                //取得使用者資料
                FormsIdentity uid = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = uid.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "DeliveryAdd", aid))
                {
                    // TODO: Add insert logic here
                    foreach (var item in collection)
                    {
                        if (item.ToString() == "REMARK" || item.ToString() == "remark")
                        {
                            order.UpdateOrder(collection["RID"].ToString(), collection["REMARK"].ToString(), aid, collection["DEPARTMENT"].ToString());
                        }
                        else if (item.ToString() != "RID" && item.ToString() != "rid" && item.ToString() != "DEPARTMENT" && item.ToString() != "department")
                        {
                            if (int.Parse(collection[item.ToString()].ToString()) > 0 && collection[item.ToString()] != null)
                            {
                                order.InsertOrderDetails(collection["RID"].ToString(), item.ToString(), collection[item.ToString()]);
                            }
                        }
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
        // GET: /DeliveryAdd/Delete

        public ActionResult Delete(int id, string chkNo)
        {
            try
            {
                // TODO: Add delete logic here
                //取得使用者資料
                FormsIdentity user = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = user.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "DeliveryAdd", aid))
                {
                    //order.DeleteOrder(id, chkNo);
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /DeliveryAdd/Deny

        public PartialViewResult Deny(int id, string chkno)
        {
            ViewData.Add("RID", id);
            ViewData.Add("CHKNO", chkno);
            return PartialView();
        }

        //
        // POST: /DeliveryAdd/Deny

        [HttpPost]
        public ActionResult Deny(int rid, string reason, string chkno)
        {
            try
            {
                // TODO: Add delete logic here
                //取得使用者資料
                FormsIdentity user = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = user.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "DeliveryAdd", aid))
                {
                    order.DenyOrder(rid, reason, chkno, aid, userid);
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
            }

            return RedirectToAction("Index");
        }

    }
}
