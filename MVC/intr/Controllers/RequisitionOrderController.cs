using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class RequisitionOrderController : Controller
    {
        //Models物件
        Models.RequisitionOrderDataContext order = new Models.RequisitionOrderDataContext();

        //
        // GET: /RequisitionOrder/

        public ActionResult Index(int id = 1, int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            //分頁
            page = (page < 1) ? 1 : page;
            int pageSize = int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"]);
            int total = 0;
            if (id == 2)
            {
                total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "CREATOR <> " + aid + " AND DID IN (SELECT DID FROM I_UD_REF01 WHERE AID=" + aid + ")"));
            }
            else
            {
                total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "CREATOR=" + aid));
            }
            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));
            List<Models.RequisitionOrder> o = order.LoadOrder(id, aid, pageSize, page);
            return View(o);
        }

        //
        // GET: /RequisitionOrder/Details/5

        public ActionResult Details(int id)
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
            {
                Models.RequisitionOrder o = order.EditOrder(aid, id);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /RequisitionOrder/Create

        public ActionResult Create()
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
            {
                Models.RequisitionOrder o = order.CreateOrder(aid);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /RequisitionOrder/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            //try
            //{
                //取得使用者資料
                FormsIdentity uid = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = uid.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
                {
                    // TODO: Add insert logic here
                    foreach (var item in collection)
                    {
                        if (item.ToString() == "REMARK" || item.ToString() == "remark")
                        {
                            order.UpdateOrder(collection["RID"].ToString(), collection["REMARK"].ToString(), collection["STATUS"].ToString(), aid, collection["DEPARTMENT"].ToString(), userid);
                        }
                        else if (item.ToString() != "RID" && item.ToString() != "rid" && item.ToString() != "STATUS" && item.ToString() != "status" && item.ToString() != "DEPARTMENT" && item.ToString() != "department")
                        {
                            if (int.Parse(collection[item.ToString()].ToString()) > 0 && collection[item.ToString()] != null)
                            {
                                order.InsertOrderDetails(collection["RID"].ToString(), item.ToString(), collection[item.ToString()]);
                            }
                        }
                    }
                }
                
            //}
            //catch (Exception e)
            //{
                //TempData.Add("error", e.Message.ToString());
            //}
            return RedirectToAction("Index");
        }

        //
        // GET: /RequisitionOrder/Edit/5

        public ActionResult Edit(int id)
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
            {
                Models.RequisitionOrder o = order.EditOrder(aid, id);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /RequisitionOrder/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                //取得使用者資料
                FormsIdentity uid = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = uid.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
                {
                    order.DeleteRequisitionDetails(id);

                    // TODO: Add insert logic here
                    foreach (var item in collection)
                    {                        
                        if (item.ToString() == "REMARK" || item.ToString() == "remark")
                        {
                            order.UpdateOrder(collection["RID"].ToString(), collection["REMARK"].ToString(), collection["STATUS"].ToString(), aid, "", userid);
                        }
                        else if (item.ToString() != "RID" && item.ToString() != "rid" && item.ToString() != "STATUS" && item.ToString() != "status" && item.ToString() != "DEPARTMENT" && item.ToString() != "department")
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
        // GET: /RequisitionOrder/Delete/5

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

                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
                {
                    order.DeleteRequisitionOrder(id);
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public PartialViewResult MaterialLists(string did, int rid = 0)
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            ViewData.Add("MaterialLists", order.GetMaterialList(aid, did, rid));
            return PartialView();
        }


        [HttpPost]
        public PartialViewResult MaterialListView(string did, int rid = 0)
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            ViewData.Add("MaterialLists", order.GetMaterialListView(aid, did, rid));
            return PartialView();
        }
    }
}
