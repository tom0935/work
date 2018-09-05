using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using System.IO;

namespace IntranetSystem.Controllers
{
    public class MaterialReportsController : Controller
    {
        Models.MaterialReportsDataContext rpt = new Models.MaterialReportsDataContext();
        //產生報表用
        public Stream stream;

        //釋放資源
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        //
        // GET: /MaterialReports/

        public ActionResult Index()
        {
            //取得使用者資料
            FormsIdentity uid = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket ticket = uid.Ticket;
            string aid = ticket.UserData;
            string userid = User.Identity.Name;

            if (User.Identity.IsAuthenticated && Models.NavigationMenu.chkUserAuth(userid, "MaterialReports", aid))
            {                
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult TonerReport()
        {
            
            return View();
        }

        //
        // GET: /MaterialReports/DeliveryReport/

        public ActionResult DeliveryReport()
        {
            ViewData.Add("MaterialList", rpt.GetMaterialDictionary());
            return View();
        }

        //
        // POST: /MaterialReports/DeliveryReportView/

        [HttpPost]
        public ActionResult DeliveryReportView(string startDate, string endDate, string mid)
        {
            List<Models.MaterialReports> m = rpt.GetDeliveryReport(startDate, endDate, mid);
            ViewData.Add("sDate", startDate);
            ViewData.Add("eDate", endDate);
            return View(m);
        }

        //
        //GET: /MaterialReports/GetInventory/

        public ActionResult GetInventory()
        {
            stream = rpt.GetInventoryStream();
            return File(stream, "application/pdf");
        }

        //部門耗材盤點表
        //GET: /MaterialReports/GetOrderSummary/

        public ActionResult GetOrderSummary()
        {
            stream = rpt.GetOrderSummaryStream();
            return File(stream, "application/pdf");
        }


    }
}
