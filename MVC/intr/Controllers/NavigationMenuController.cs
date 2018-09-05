using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class NavigationMenuController : Controller
    {
        //
        // POST: /NavigationMenu/NavMenu

        [HttpPost]
        public PartialViewResult NavMenu()
        {
            //取得應用程式網址
            string url = Request.ApplicationPath.ToString();
            url = (url == "/") ? "" : url;
            try
            {
                //取得使用者資料
                FormsIdentity user = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = user.Ticket;
                string aid = ticket.UserData;
                string userid = User.Identity.Name;

                //取得選單
                ViewData.Add("DepartmentMsg", Models.NavigationMenu.getDepartmentData(aid));
                ViewData.Add("NavMenu", Models.NavigationMenu.getNavMenu(aid, url));
                ViewData.Add("AID", aid);
            }
            catch
            {
                //取得選單
                ViewData.Add("DepartmentMsg", "");
                ViewData.Add("NavMenu", "");
            }
            return PartialView();
        }
    }
}
