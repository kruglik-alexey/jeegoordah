using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Jeegoordah.Core.BL;
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
                var currencies = db.Query<Currency>().OrderBy(c => c.Name).ToList().Select(c => new CurrencyRest(c)).ToList();
                // sanity check
                currencies.Single(c => c.IsBase);
                return Json(currencies, JsonRequestBehavior.AllowGet);
            }
        }        

        [HttpPost]
        async public Task<ActionResult> UpdateExchangeRates()
        {
            using (var db = DbFactory.Open())
            {
                Logger.I("Begin updating exchange rates");            
                await ExchangeRatesUpdater.Update(db);   
                db.Commit();
                Logger.I("Updating exchange rates complete");
                return Json(new { });
            }            
        }

        [HttpGet]
        public ActionResult GetRates(string date)
        {
            var d = JsonDate.Parse(date);
            using (var db = DbFactory.Open())
            {
                return Json(db.Query<ExchangeRate>().Where(r => r.Date == d)
                    .ToList()
                    .Select(r => new {r.Rate, Date = JsonDate.ToString(r.Date), Currency = r.Currency.Id})
                    .ToList(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
