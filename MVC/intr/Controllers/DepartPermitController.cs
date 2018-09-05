using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IntranetSystem.Controllers
{
    public class DepartPermitController : Controller
    {
        //Models物件
        Models.DepartPermitDataContext dpermit = new Models.DepartPermitDataContext();

        //
        // GET: /DepartPermit/Edit/5

        public ActionResult Edit(int id)
        {
            Models.DepartPermit d = dpermit.LoadDepartPermit(id);
            return View(d);
        }

        //
        // POST: /DepartPermit/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.DepartPermit dp = new Models.DepartPermit();
                dp.DID = (decimal)id;
                dp.GMapping = collection["HASGROUP"];
                dp.PMapping = collection["HASPROGRAM"];
                dpermit.UpdateDepartPermit(dp);
                Models.DepartPermit d = dpermit.LoadDepartPermit(id);
                ViewData.Add("message", "儲存完畢!");
                return View(d);
            }
            catch (Exception e)
            {
                Models.DepartPermit d = dpermit.LoadDepartPermit(id);
                ViewData.Add("error", e.Message.ToString());
                return View(d);
            }
        }

    }
}
