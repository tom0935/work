using System;
using System.Web.Mvc;

namespace IntranetSystem.Controllers
{
    public class SearchMaterialController : Controller
    {
        //Models物件
        Models.SearchMaterial search = new Models.SearchMaterial();

        //
        // POST: /SearchMaterial/TypeLists/5

        [HttpPost]
        public PartialViewResult TypeLists(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return PartialView();
            }
            catch
            {
                return PartialView();
            }
        }


        //
        // POST: /SearchMaterial/MaterialLists/5

        [HttpPost]
        public ContentResult MaterialLists(string type)
        {
            try
            {
                // TODO: Add update logic here
                string lists = search.getMaterialLists(type);
                return Content(lists, "text/html");
            }
            catch(Exception e)
            {
                return Content(e.Message.ToString());
            }
        }
    }
}
