using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Jeegoordah.Core.DL;
using Jeegoordah.Core.Logging;
using Jeegoordah.Web.Controllers;
using Jeegoordah.Web.DL;
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
            var logger = Logger.For(this);
			logger.I("Start Jeegoordah for the greater good!");
            AppDomain.CurrentDomain.DomainUnload += (_, __) => logger.I("Unloaded");

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            ControllerBuilder.Current.SetControllerFactory(ObjectFactory.Container.GetInstance<ControllerFactory>());
            GlobalConfiguration.Configuration.BindParameter(typeof(DateTime), new DateTimeModelBinder());
            
            RunBackup();                
        }

        private void RunBackup()
        {
            Timer timer = null;
            timer = new Timer(s =>
            {
                timer.Dispose();
                // path to AppData directory
                string appDataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
                DbBackup.Backup(ConfigurationManager.ConnectionStrings["Jeegoordah"].ConnectionString, appDataDirectory);
            }, null, TimeSpan.FromSeconds(10), TimeSpan.Zero);             
        }

        private void ConfigureLogger()
	    {
			Logger.Configure(@"App_Data\log.txt");
			log4net.Config.XmlConfigurator.Configure();
	    }
    }
}