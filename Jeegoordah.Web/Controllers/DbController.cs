using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core.DL;
using Jeegoordah.Core.Logging;
using Jeegoordah.Web.DL;

namespace Jeegoordah.Web.Controllers
{
	public abstract class DbController : BaseController
    {	    
        protected readonly ContextDependentDbFactory DbFactory;

        protected DbController(ContextDependentDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }		
    }
}