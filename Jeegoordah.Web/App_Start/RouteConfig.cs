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

            #region Events

            routes.MapRoute(
                name: "UpdateEvent",
                url: "events/update",
                defaults: new { controller = "Events", action = "Update" }
            );
            routes.MapRoute(
                name: "DeleteEvent",
                url: "events/delete/{id}",
                defaults: new { controller = "Events", action = "Delete" }
            );
            routes.MapRoute(
                name: "CreateEvent",
                url: "events/create",
                defaults: new { controller = "Events", action = "Create" }
            );
            routes.MapRoute(
                name: "ListEvents",
                url: "events",
                defaults: new { controller = "Events", action = "List" }
            );
            routes.MapRoute(
                name: "GetEvent",
                url: "events/{id}",
                defaults: new { controller = "Events", action = "Get" }
            );

            routes.MapRoute(
                name: "GetEventTransactions",
                url: "events/{id}/transactions",
                defaults: new { controller = "Events", action = "GetTransactions" }
            );

            #endregion

            #region Transactions

            routes.MapRoute(
                name: "CreateTransaction",
                url: "transactions/create",
                defaults: new { controller = "Transactions", action = "Create" }
            );

            routes.MapRoute(
                name: "UpdateTransaction",
                url: "transactions/update",
                defaults: new { controller = "Transactions", action = "Update" }
            );

            routes.MapRoute(
                name: "DeleteTransaction",
                url: "transactions/delete/{id}",
                defaults: new { controller = "Transactions", action = "Delete" }
            );

            routes.MapRoute(
                name: "ListP2PTransactions",
                url: "transactions/p2p",
                defaults: new { controller = "Transactions", action = "ListP2P" }
            );

            #endregion

            #region General

            routes.MapRoute(
                name: "ListBros",
                url: "bros",
                defaults: new { controller = "General", action = "ListBros" }
            );

            routes.MapRoute(
                name: "ListCurrensies",
                url: "currencies",
                defaults: new { controller = "General", action = "ListCurrencies" }
            );

            routes.MapRoute(
                name: "GetTotal",
                url: "total",
                defaults: new { controller = "General", action = "GetTotal" }
            );

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "General", action = "Index" }
            );

            #endregion

#if DEBUG
            routes.MapRoute(
                name: "ClearDatabase",
                url: "test/cleardatabase",
                defaults: new { controller = "Test", action = "ClearDatabase" }
            );
#endif
        }
    }
}