using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IntranetSystem.Controllers
{
    public class UserPermitController : Controller
    {
        //Models物件
        Models.UserPermitDataContext upermit = new Models.UserPermitDataContext();

        //
        // GET: /UserPermit/Edit/5

        public ActionResult Edit(int id)
        {
            Models.UserPermit p = upermit.LoadUserPermit(id);
            return View(p);
        }

        //
        // POST: /UserPermit/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.UserPermit up = new Models.UserPermit();
                up.AID = (decimal)id;
                up.GMapping = collection["HASGROUP"];
                up.PMapping = collection["HASPROGRAM"];
                upermit.UpdateUserPermit(up);
                Models.UserPermit p = upermit.LoadUserPermit(id);
                ViewData.Add("message", "儲存完畢!");
                return View(p);
            }
            catch (Exception e)
            {
                Models.UserPermit p = upermit.LoadUserPermit(id);
                ViewData.Add("error", e.Message.ToString());
                return View(p);
            }
        }

    }
}
