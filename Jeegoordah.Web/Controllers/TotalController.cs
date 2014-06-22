using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core.BL;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.DL;
using Jeegoordah.Web.Models;

namespace Jeegoordah.Web.Controllers
{
    public class TotalController : DbController
    {
        public TotalController(ContextDependentDbFactory dbFactory) : base(dbFactory)
        {
        }

        [HttpGet]
        public ActionResult GetTotal()
        {
            using (var db = DbFactory.Open())
            {
                Dictionary<Bro, Dictionary<Currency, decimal>> total = TotalCalculator.Calculate(db.Query<Transaction>().ToList(), db.Query<Bro>().ToList());
                var result = total.Keys.Select(bro => new BroTotalRest(bro, total[bro])).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTotalInBaseCurrency()
        {
            using (var db = DbFactory.Open())
            {
                Dictionary<Bro, decimal> total = TotalCalculator.CalculateInBaseCurrency(db.Query<Transaction>().ToList(), db.Query<Bro>().ToList());
                var result = total.Keys.Select(bro => new BroTotalInCurrencyRest(bro, total[bro])).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }        
    }
}
