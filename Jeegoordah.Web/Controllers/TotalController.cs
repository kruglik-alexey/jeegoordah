using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jeegoordah.Core;
using Jeegoordah.Core.BL;
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Web.DL;
using Jeegoordah.Web.Models;
using NHibernate.Linq;

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
        public ActionResult GetTotalInCurrency(int currencyId)
        {
            using (var db = DbFactory.Open())
            {
                var now = DateTime.UtcNow.Date;
                var rate = ExchangeRateProvider.Get(db.Query<ExchangeRate>(), currencyId, now);
                
                Dictionary<Bro, decimal> total = TotalCalculator.CalculateInCurrency(rate, db.Query<Transaction>().ToList(), db.Query<Bro>().ToList());
                var broTotalsRest = total.Keys.Select(bro => new BroTotalInCurrencyRest(bro, total[bro])).ToList();
                return Json(new TotalInCurrencyRest(broTotalsRest, rate), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
