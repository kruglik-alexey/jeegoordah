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
	            List<Transaction> transactions;
	            List<Bro> bros;
				Dictionary<Bro, Dictionary<Currency, decimal>> total;

				using (new PerfCounter("total-transactions")) transactions = db.Query<Transaction>().ToList();
	            using (new PerfCounter("total-bros")) bros = db.Query<Bro>().ToList();
				using (new PerfCounter("total-calcall")) total = TotalCalculator.Calculate(transactions, bros);

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

				Transaction[] transactions;
				Bro[] bros;
				Dictionary<Bro, decimal> total;
	            ExchangeRate rate;

				using (new PerfCounter("total-transactions"))
				{
					// loading transactions and their targets with one query to avoid N+1 query for each transaction targets
					transactions = db.Query<Transaction>()
						.SelectMany(t => t.Targets, (transaction, bro) => new {transaction, bro})
						.AsEnumerable()
						.GroupBy(p => p.transaction, p => p.bro, (transaction, targets) =>
						{
							transaction.Targets = targets.ToArray();
                            return transaction;
						})
						.ToArray();
				}

				using (new PerfCounter("total-bros")) bros = db.Query<Bro>().ToArray();
				using (new PerfCounter("total-rate")) rate = ExchangeRateProvider.Get(db.Query<ExchangeRate>(), currencyId, now);
				using (new PerfCounter("total-calc")) total = TotalCalculator.CalculateInCurrency(rate, transactions, bros);
                var broTotalsRest = total.Keys.Select(bro => new BroTotalInCurrencyRest(bro, total[bro])).ToList();
                return Json(new TotalInCurrencyRest(broTotalsRest, rate), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
