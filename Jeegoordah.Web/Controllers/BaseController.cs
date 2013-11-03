using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core.Logging;

namespace Jeegoordah.Web.Controllers
{
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
}