using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Web.Models;

namespace Jeegoordah.Web.Controllers
{
    public class BrosController : DbController
    {
        [HttpGet]
        public ActionResult List()
        {
            using (var db = DbFactory.CreateDb())
            {
                return Json(db.Bros.OrderBy(b => b.Name).ToList().Select(b => new BroRest(b)), JsonRequestBehavior.AllowGet);
            }
        }
    }
}