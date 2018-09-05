using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IntranetSystem.Controllers
{
    public class HomeController : Controller
    {
        //Models物件,調整位置
        Models.Bulletin.Dashboards dbs = new Models.Bulletin.Dashboards();


        //
        // GET: /Home/

        public ActionResult Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    //取得使用者資料
                    FormsIdentity id = (FormsIdentity)User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    string aid = ticket.UserData;
                    string userid = User.Identity.Name;

                    //ViewData.Add("AID", aid);

                    //耗材審核通知
                    if (Models.NavigationMenu.chkUserAuth(userid, "RequisitionAudit", aid))
                    {
                        ViewData.Add("AuditMsg", dbs.getAuditData(aid));
                        ViewData.Add("AuditOk", "YES");
                    }
                    //耗材不足通知
                    if (Models.NavigationMenu.chkUserAuth(userid, "Material", aid))
                    {
                        ViewData.Add("MaterialMsg", dbs.getMaterialData());
                        ViewData.Add("MaterialMsgCount", dbs.getMaterialData().Count);
                    }
                    //耗材領料通知IMC
                    if (Models.NavigationMenu.chkUserAuth(userid, "DeliveryOrder", aid))
                    {
                        ViewData.Add("DeliveryMsg", dbs.getDeliveryData());
                    }
                    //耗材領料通知USER
                    if (Models.NavigationMenu.chkUserAuth(userid, "RequisitionOrder", aid))
                    {
                        ViewData.Add("RequisitionMsg", dbs.getRequisitionData(aid));
                    }

                    //安全聯防通報
                    Models.Bulletin.Bulletin blt = new Models.Bulletin.Bulletin();
                    List<Poco.BulletinPoco> sbd = blt.LoadBulletin(1, 1, 10);
                    List<Poco.BulletinPoco> doc = blt.LoadBulletin(2, 1, 10);
                                        
                    Poco.BulletinViewModels bltViewModels = new Poco.BulletinViewModels();
                    bltViewModels.SBD = sbd;
                    bltViewModels.DOC = doc;
                    string ip = Request.UserHostAddress;
                    ViewData.Add("MacList", blt.GetMacAddress(ip));
                    return View(bltViewModels);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch
            {
                //清空所有Session、Cookie 資料
                Session.RemoveAll();
                Response.Cookies.Clear();
                return RedirectToAction("Login");
            }
        }

        //
        // GET: /Home/Login

        public ActionResult Login()
        {
            //安全聯防通報
            Models.Bulletin.Bulletin blt = new Models.Bulletin.Bulletin();
            List<Poco.BulletinPoco> sbd = blt.LoadBulletin(1, 1, 10);
            List<Poco.BulletinPoco> doc = blt.LoadBulletin(2, 1, 10);

            Poco.BulletinViewModels bltViewModels = new Poco.BulletinViewModels();
            bltViewModels.SBD = sbd;
            bltViewModels.DOC = doc;
            return View(bltViewModels);
        }

        //
        // POST: /Home/Login

        [HttpPost]
        public ActionResult Login(string userid, string passwd, string remember)
        {
            Models.Login login = new Models.Login();
            try
            {
                bool result = login.chkUser(userid.Trim(), passwd.Trim());
                if (result)
                {
                    // 登入前清空所有Session、Cookie 資料
                    Session.RemoveAll();
                    Response.Cookies.Clear();

                    bool isPersistent = (remember == "True") ? true : false;
                    int Expiration = (isPersistent) ? (60 * 24 * 7) : 60;

                    // 將存儲在 Cookie 中的用戶定義數據。
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,                          // 版本號。
                        userid,                             // 與身份驗證票關聯的用戶名。
                        DateTime.Now,                       // Cookie 的發出時間。
                        DateTime.Now.AddMinutes(Expiration),// Cookie 的到期日期。
                        isPersistent,                       // 如果 Cookie 是持久的，為 true；否則為 false。
                        login.AID.ToString(),                         // userData 使用者定義資料
                        FormsAuthentication.FormsCookiePath // Cookie路徑
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket); //加密

                    //存入Cookie
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    cookie.Expires = DateTime.Now.AddMinutes(Expiration);
                    Response.Cookies.Add(cookie);

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData.Add("error", "Oh~No!! 有錯誤，請重新登入!!");
                    return RedirectToAction("Login");
                }
            }
            catch (Exception e)
            {
                TempData.Add("error", e.Message.ToString());
                return RedirectToAction("Login");
            }
        }

        //
        // GET: /Home/Logout

        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData.Add("message", User.Identity.Name + "已登出。");
                FormsAuthentication.SignOut();
            }
            return RedirectToAction("Login");
        }
    }
}
