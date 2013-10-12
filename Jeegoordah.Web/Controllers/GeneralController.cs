using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Jeegoordah.Web.Models;

namespace Jeegoordah.Web.Controllers
{
    public class GeneralController : DbController
    {      
        [HttpGet]
        public ActionResult Index()
        {
            return View(new CacheBuster());
        }

        [HttpGet]
        public ActionResult ListBros()
        {
            using (var db = DbFactory.CreateDb())
            {
                return Json(db.Bros.OrderBy(b => b.Name).ToList().Select(b => new BroRest(b)), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListCurrencies()
        {
            using (var db = DbFactory.CreateDb())
            {
                return Json(db.Currencies.OrderBy(c => c.Name).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
