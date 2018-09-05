using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class DeliveryOrderController : Controller
    {
        //Models物件
        Models.DeliveryOrderDataContext order = new Models.DeliveryOrderDataContext();

        //
        // GET: /DeliveryOrder/

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
            switch (id)
            {
                default:
                case 1: //待領用
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=3"));
                    break;
                case 2: //已領用
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=4"));
                    break;
                case 3: //待審中
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=2"));
                    break;
            }

            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));
            List<Models.DeliveryOrder> o = order.LoadOrder(id, aid, pageSize, page);
            return View(o);
        }

        //
        // GET: /DeliveryOrder/Details/5

        public ActionResult Details(int id)
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "DeliveryOrder", aid))
            {
                Models.DeliveryOrder o = order.EditOrder(aid, id);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /DeliveryOrder/Deny/5

        public ActionResult Deny(int id, string chkno, string aid = "")
        {
            if (aid == "")
            {
                //取得使用者資料
                FormsIdentity uid = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = uid.Ticket;
                aid = ticket.UserData;
            }

            try
            {
                int status = int.Parse(Models.Helper.getFieldValue("STATUS", "I_REQUISITIONORDER", "RID=" + id));

                if (status < 4)
                {
                    if (order.DenyOrder(id, chkno, aid))
                        TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已取消申請!");
                    else
                        TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 無動作!");
                }
                else
                {
                    TempData.Add("error", "單號" + id.ToString().PadLeft(8, '0') + " 已領用，無法取消");
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
            }
            return RedirectToAction("Index");
        }

    }
}
