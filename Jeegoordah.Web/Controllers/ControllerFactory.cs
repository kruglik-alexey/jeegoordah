using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Jeegoordah.Core.DL;
using StructureMap;

namespace Jeegoordah.Web.Controllers
{
    public class ControllerFactory : DefaultControllerFactory
    {
        private readonly IContainer container;
        private readonly object nestedContainerKey = new object();

        public ControllerFactory(IContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {            
            var nestedContainer = container.GetNestedContainer();
            requestContext.HttpContext.Items[nestedContainerKey] = nestedContainer;

            nestedContainer.Configure(cfg =>
            {
                cfg.For<RequestContext>().Use(requestContext);
            });

            return (IController)nestedContainer.GetInstance(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            var nestedContainer = (IContainer)((ControllerBase)controller).ControllerContext.HttpContext.Items[nestedContainerKey];
            if (nestedContainer != null)
                nestedContainer.Dispose();            
            base.ReleaseController(controller);
        }
    }
}