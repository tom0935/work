using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace IntranetSystem.Controllers
{
    public class DepartMappingController : Controller
    {
        //Models物件
        Models.DepartMappingDataContext dm = new Models.DepartMappingDataContext();

        //
        // GET: /DepartMapping/Edit/5

        public ActionResult Edit(int id)
        {
            Models.DepartMapping d = dm.LoadDepartMapping(id);
            return View(d);
        }

        //
        // POST: /DepartMapping/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.DepartMapping d = new Models.DepartMapping();
                d.AID = (decimal)id;
                d.Mapping = collection["WORKIN"];
                dm.UpdateDepartMapping(d);
                Models.DepartMapping ldm = dm.LoadDepartMapping(id);
                ViewData.Add("message", "儲存完畢!");
                return View(ldm);
            }
            catch(Exception e)
            {
                Models.DepartMapping d = dm.LoadDepartMapping(id);
                ViewData.Add("error", e.Message.ToString());
                return View(d);
            }
        }

    }
}
