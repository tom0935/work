using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class RequisitionAuditController : Controller
    {
        //Models物件
        Models.RequisitionAuditDataContext order = new Models.RequisitionAuditDataContext();

        //
        // GET: /RequisitionAudit/

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
                case 1: //待審核
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=2"));
                    break;
                case 2: //已審核
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS>2"));
                    break;
                case 3: //作廢
                    total = int.Parse(Models.Helper.getFieldValue("count(*)", "I_REQUISITIONORDER", "STATUS=0"));
                    break;
            }

            ViewData.Add("pageList", IntranetSystem.Models.PageList.PageString(total, pageSize, page));
            List<Models.RequisitionAudit> o = order.LoadOrder(id, aid, pageSize, page);
            return View(o);
        }

        //
        // GET: /RequisitionAudit/Details/5

        public ActionResult Details(int id)
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "RequisitionAudit", aid))
            {
                Models.RequisitionOrderDataContext order = new Models.RequisitionOrderDataContext();
                Models.RequisitionOrder o = order.EditOrder(aid, id);
                return View(o);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /RequisitionAudit/Allow/5

        public ActionResult Allow(int id, string chkno, string aid = "")
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
                int status = int.Parse(Models.Helper.getFieldValue("STATUS", "I_REQUISITIONORDER", "RID=" + id + " AND CHKNO='" + chkno + "'"));

                if (status == 1)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 為暫存新單狀態，無法審核!");
                }
                else if (status == 2)
                {

                    if (order.AllowOrder(id, chkno, aid))
                        TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已審核完成!");
                    else
                        TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 無動作!");
                }
                else if (status == 3)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已審核通過，無法重複審核!");
                }
                else if (status == 4)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已領料完成，無法審核!");
                }
                else if (status == 0)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已作廢，無法審核!");
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
            }
            return RedirectToAction("Index", new { id = 2 });
        }

        //
        // GET: /RequisitionAudit/Deny/5

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
                //int status = int.Parse(Models.Helper.getFieldValue("STATUS", "I_REQUISITIONORDER", "RID=" + id));
                int status = int.Parse(Models.Helper.getFieldValue("STATUS", "I_REQUISITIONORDER", "RID=" + id + " AND CHKNO='" + chkno + "'"));

                if (status == 1)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 為暫存新單狀態，無法取消!");
                }
                else if (status == 2)
                {
                    if (order.DenyOrder(id, chkno, aid))
                        TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已取消申請!");
                    else
                        TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 無動作!");
                }
                else if (status == 3)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已審核完成，無法取消!");
                }
                else if (status == 4)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已領料完成，無法取消!");
                }
                else if (status == 0)
                {
                    TempData.Add("message", "單號" + id.ToString().PadLeft(8, '0') + " 已作廢!");
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message);
            }
            return RedirectToAction("Index", new { id = 3 });
        }

    }
}
