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
                var currency = db.Load<Currency>(currencyId);
                var rate = db.Query<ExchangeRate>().FirstOrDefault(r => r.Date == now && r.Currency == currency);
                if (rate == null)
                {
                    if (currency.IsBase())
                    {
                        rate = new ExchangeRate {Currency = currency, Date = now, Rate = 1};
                    }
                    else
                    {
                        throw new InvalidOperationException("Has no rate for {0} on {1}".F(currency.Name, now));                        
                    }
                }

                Dictionary<Bro, decimal> total = TotalCalculator.CalculateInCurrency(rate, db.Query<Transaction>().ToList(), db.Query<Bro>().ToList());
                var broTotalsRest = total.Keys.Select(bro => new BroTotalInCurrencyRest(bro, total[bro])).ToList();
                return Json(new TotalInCurrencyRest(broTotalsRest, currency, rate), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
