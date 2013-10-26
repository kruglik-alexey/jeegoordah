using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.Controllers
{
    public class ControllerFactory : DefaultControllerFactory
    {
        private readonly Type dbControllerType = typeof(DbController);
        private readonly Lazy<DbFactory> realDbFactory = new Lazy<DbFactory>(() => new DbFactory("Jeegoordah"));
        private readonly Lazy<DbFactory> testDbFactory = new Lazy<DbFactory>(() => new DbFactory("JeegoordahTest"));

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var controller = Activator.CreateInstance(controllerType);

            if (dbControllerType.IsAssignableFrom(controllerType))
            {
#if DEBUG
                var dbFactory = requestContext.HttpContext.Request.Params["test"] == null
                    ? realDbFactory.Value
                    : testDbFactory.Value;
                ((DbController)controller).SetDbFactory(dbFactory);   
#else
                ((DbController)controller).SetDbFactory(realDbFactory.Value); 
#endif
            }            

            return (IController)controller;
        }
    }
}