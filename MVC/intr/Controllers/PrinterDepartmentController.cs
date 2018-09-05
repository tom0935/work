using System.Web.Mvc;
using System.Web.Security;
using System;

namespace IntranetSystem.Controllers
{
    public class PrinterDepartmentController : Controller
    {
        //Models物件
        Models.PrinterDepartmentDataContext pd = new Models.PrinterDepartmentDataContext();

        //
        // GET: /PrinterDepartment/Edit/5

        public ActionResult Edit(int id)
        {
            Models.PrinterDepartment p = pd.GetPrinter(id);
            ViewData.Add("DepartmentLists", pd.DepartmentlLists());
            ViewData.Add("Details", pd.Details(id));
            return View(p);
        }

        //
        // POST: /PrinterDepartment/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {

            // TODO: Add insert logic here
            //取得使用者資料
            FormsIdentity user = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = user.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;
            string ignoreText = "TtSs";
            try
            {
                if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "PrinterDepartment", aid))
                {
                    pd.DeletePrinterDepartment(id);
                }

                foreach (var item in collection)
                {
                    if (!ignoreText.Contains(item.ToString().Substring(item.ToString().Length - 1)))
                    {
                        int noshow = (collection[item.ToString() + "NS"] == null) ? 0 : 1;
                        pd.UpdatePrinterDepartment(collection[item.ToString()], id, collection[item.ToString() + "T"], noshow);
                    }
                }
                return RedirectToAction("Index", "Material");
            }
            catch (Exception e)
            {
                Models.PrinterDepartment p = pd.GetPrinter(id);
                ViewData.Add("DepartmentLists", pd.DepartmentlLists());
                ViewData.Add("Details", pd.Details(id));
                ViewData.Add("message", e.Message);
                return View(p);
            }
        }

    }
}
