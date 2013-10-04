﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.Controllers
{
    public class ControllerFactory : DefaultControllerFactory
    {
        private readonly Type _dbControllerType = typeof(DbController);

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var controller = Activator.CreateInstance(controllerType);
#if DEBUG
            if (_dbControllerType.IsAssignableFrom(controllerType))
            {
                var dbFactory = requestContext.HttpContext.Request.Params["test"] == null
                   ? new DbFactory(typeof(JeegoordahRealDb))
                   : new DbFactory(typeof(JeegoordahTestDb));
                ((DbController)controller).SetDbFactory(dbFactory);   
            }            
#endif
            return (IController)controller;
        }
    }
}