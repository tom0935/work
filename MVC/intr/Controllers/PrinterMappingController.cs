using System.Web.Mvc;
using System;

namespace IntranetSystem.Controllers
{
    public class PrinterMappingController : Controller
    {
        //Models物件
        Models.PrinterMappingDataContext pm = new Models.PrinterMappingDataContext();

        //
        // GET: /PrinterMapping/Edit/5

        public ActionResult Edit(int id)
        {
            Models.PrinterMapping p = pm.LoadPrinterMapping(id);
            return View(p);
        }

        //
        // POST: /PrinterMapping/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.PrinterMapping p = new Models.PrinterMapping();
                p.PRINTER = (decimal)id;
                p.Mapping = collection["HASTONER"];
                pm.UpdatePrinterMapping(p);
                Models.PrinterMapping upm = pm.LoadPrinterMapping(id);
                ViewData.Add("message", "儲存完畢!");
                return View(upm);
            }
            catch (Exception e)
            {
                Models.PrinterMapping upm = pm.LoadPrinterMapping(id);
                ViewData.Add("error", e.Message.ToString());
                return View(upm);
            }
        }

    }
}
