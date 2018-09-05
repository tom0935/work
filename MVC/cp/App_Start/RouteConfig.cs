using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HowardCoupon_vs2010
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("Page/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.htm/{*pathInfo}");
            
            
  
          //  routes.IgnoreRoute("*.html");
            

            routes.MapRoute(
                name: "Maintain",
                url: "Maintain/{controller}/{action}/{id}",
                defaults: new { controller = "Company", action = "Index", id = UrlParameter.Optional }               
            );

            routes.MapRoute(
                name: "Isu",
                url: "Isu/{controller}/{action}/{id}",
                defaults: new { controller = "Isu", action = "Index", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}