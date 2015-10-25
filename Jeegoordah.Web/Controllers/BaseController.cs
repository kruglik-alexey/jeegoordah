using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Jeegoordah.Core.Logging;
using NHibernate.Util;

namespace Jeegoordah.Web.Controllers
{
	[AuthorizationRequired]
	abstract public class BaseController : Controller
	{
		protected readonly Logger Logger;

		protected BaseController()
		{
			Logger = Logger.For(this);
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			Logger.E(filterContext.Exception);
		}
	}

	public class NoAuthorizationRequired : Attribute
	{		
	}

	public class AuthorizationRequired : ActionFilterAttribute
	{
		public const string CookieName = "FB83D6D9-6D18-419E-B862-1A1DBE30F536";
		public const string Password = "fortytwo";

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			if (filterContext.ActionDescriptor.GetCustomAttributes(false).OfType<NoAuthorizationRequired>().Any())
			{
				return;
			}
            if (!filterContext.HttpContext.Request.Cookies.AllKeys.Contains(CookieName))
			{
				filterContext.Result = new RedirectToRouteResult("Login", new RouteValueDictionary());
			}
        }
	}
}