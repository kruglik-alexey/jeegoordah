using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Jeegoordah.Web.DL;
using NHibernate.Linq;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.Models;

namespace Jeegoordah.Web.Controllers
{
    public class GeneralController : DbController
    {
        public GeneralController(ContextDependentDbFactory dbFactory) : base(dbFactory)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {            
            return View(new CacheBuster());
        }

        [HttpGet]
        public ActionResult ListBros()
        {
            using (var db = DbFactory.Open())
            {
                return Json(db.Query<Bro>().OrderBy(b => b.Name).ToList().Select(b => new BroRest(b)), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListCurrencies()
        {
            using (var db = DbFactory.Open())
            {
                return Json(db.Query<Currency>().OrderBy(c => c.Name).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
