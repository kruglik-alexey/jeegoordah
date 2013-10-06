using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Jeegoordah.Web.Controllers
{
    public class GeneralController : DbController
    {      
        [HttpGet]
        public ActionResult Index()
        {
            return View(new CacheBuster());
        }        
    }
}
