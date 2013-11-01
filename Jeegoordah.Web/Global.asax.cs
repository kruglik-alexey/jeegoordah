using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Jeegoordah.Core.Logging;
using Jeegoordah.Web.Controllers;
using Jeegoordah.Web.Models.Validation;
using StructureMap;

namespace Jeegoordah.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			ConfigureLogger();
			Logger.For(this).I("Start Jeegoordah for the greater good!");

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ControllerBuilder.Current.SetControllerFactory(ObjectFactory.Container.GetInstance<ControllerFactory>());
            GlobalConfiguration.Configuration.BindParameter(typeof(DateTime), new DateTimeModelBinder());                        
        }

	    private void ConfigureLogger()
	    {
			Logger.Configure(@"App_Data\log.txt");
			log4net.Config.XmlConfigurator.Configure();
	    }
    }
}