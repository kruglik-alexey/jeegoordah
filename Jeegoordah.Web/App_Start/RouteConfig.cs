using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jeegoordah.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UpdateEvent",
                url: "events/update",
                defaults: new { controller = "General", action = "UpdateEvent" }
            );
            routes.MapRoute(
                name: "DeleteEvent",
                url: "events/delete/{id}",
                defaults: new { controller = "General", action = "DeleteEvent" }
            );
            routes.MapRoute(
                name: "CreateEvent",
                url: "events/create",
                defaults: new { controller = "General", action = "CreateEvent" }
            );
            routes.MapRoute(
                name: "Events",
                url: "events",
                defaults: new { controller = "General", action = "Events" }
            );


#if DEBUG
            routes.MapRoute(
                name: "ClearDatabase",
                url: "test/cleardatabase",
                defaults: new { controller = "Test", action = "ClearDatabase" }
            );  
#endif

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "General", action = "Index", id = UrlParameter.Optional }
            );            
        }
    }
}