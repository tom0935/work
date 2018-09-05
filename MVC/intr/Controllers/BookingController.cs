using System.Web.Mvc;
using IntranetSystem.Models.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IntranetSystem.Controllers
{
    public class BookingController : Controller
    {
        //Model
        private BookingWebFdService bws = new BookingWebFdService();

        //
        // GET: /Booking/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /BookingTool/GetGrid

        public ActionResult GetGrid()
        {
            JObject jo = bws.getTransData();
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //
        // GET: /Booking/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Booking/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Booking/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Booking/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Booking/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Booking/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Booking/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // POST: /Booking/CheckPid

        [HttpPost]
        public ActionResult RemovePid(string pid)
        {
            try
            {
                // TODO: Add Remove logic here
                string status = bws.removePid(pid);
                return Content(status);
            }
            catch
            {
                return Content("XX");
            }
        }

        public ActionResult Test()
        {
            string adminMail = System.Configuration.ConfigurationManager.AppSettings["AdminMail"].ToString();
            return Content(adminMail);
        }


    }
}
