using Jasper.Models;
using Jasper.Models.Poco;
using Jasper.service;
using Jasper.service.frm3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jasper.Controllers.frm3
{

    public class Frm304Controller : Controller
    {
        Frm304Service f304service = new Frm304Service();
        //
      

        public ActionResult frm304()
        {
            return View();
        }

        public ActionResult CustomerDialog(String CSTA)
        {
            if (StringUtils.getString(CSTA) != "")
            {
                ViewBag.CSTA = CSTA.Trim();
            }
            return View();
        }


        public ActionResult frm201(String RMNO, String CNHNO, String CSTA)
        {

            if (StringUtils.getString(RMNO) != "")
            {
                ViewBag.RMNO = RMNO.Trim();
            }
            if (StringUtils.getString(CNHNO) != "")
            {
                ViewBag.CNHNO = CNHNO.Trim();
            }
            if (StringUtils.getString(CSTA) != "")
            {
                ViewBag.CSTA = CSTA.Trim();
            }
            return View();
        }


        public ActionResult Grid(Frm304Poco param)
        {
            JObject jo = new JObject();
            try
            {                
                jo= f304service.getDatagrid(param);
                jo.Add("status", "1");
            }
            catch (Exception e)
            {
                jo.Add("status", "0");
                jo.Add("message", e.StackTrace);
            }
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }




    }
}
