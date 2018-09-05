using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace IntranetSystem.Controllers
{
    public class CompanyController : Controller
    {
        //Models物件
        Models.CompanyDataContext com = new Models.CompanyDataContext();

        //
        // GET: /Company/

        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
         
            Models.Company company = com.LoadCompany();
            return View(company);
        }

        //
        // GET: /Company/Edit/5

        public ActionResult Edit(int id)
        {
            Models.Company company = com.LoadCompany();
            return View(company);
        }

        //
        // POST: /Company/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Models.Company c = new Models.Company();
                c.CODE = collection["CODE"];
                c.NAME = collection["NAME"];
                c.VAT = collection["VAT"];
                c.ADDR = collection["ADDR"];
                c.TEL = collection["TEL"];
                c.FAX = collection["FAX"];
                c.URL = collection["URL"];
                c.REMARK = collection["REMARK"];
                com.UpdateCompany(c);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ViewData.Add("error", e.Message.ToString());
                return View();
            }
        }
    }
}
