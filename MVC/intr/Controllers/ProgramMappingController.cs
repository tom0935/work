using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetSystem.Controllers
{
    public class ProgramMappingController : Controller
    {

        //Models物件
        Models.ProgramMappingDataContext pm = new Models.ProgramMappingDataContext();

        //
        // GET: /ProgramMapping/Edit/5

        public ActionResult Edit(int id)
        {
            Models.ProgramMapping p = pm.LoadProgramMapping(id);
            return View(p);
        }

        //
        // POST: /ProgramMapping/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.ProgramMapping p = new Models.ProgramMapping();
                p.GID = (decimal)id;
                
                p.Mapping = collection["HASPROGRAM"];
                pm.UpdateProgramMapping(p);
                Models.ProgramMapping lpm = pm.LoadProgramMapping(id);
                ViewData.Add("message", "儲存完畢!");
                return View(lpm);
            }
            catch (Exception e)
            {
                Models.ProgramMapping p = pm.LoadProgramMapping(id);
                ViewData.Add("error", e.Message.ToString());
                return View(p);
            }
        }

    }
}
