using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
		[NoAuthorizationRequired]
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[NoAuthorizationRequired]
		public ActionResult Login(FormCollection form)
		{
			if (form["foo"] == AuthorizationRequired.Password)
			{
				HttpCookie cookie = new HttpCookie(AuthorizationRequired.CookieName, "1")
				{
					Expires = DateTime.UtcNow.AddYears(1)
				};
				HttpContext.Response.Cookies.Add(cookie);
				return new RedirectToRouteResult("Default", new RouteValueDictionary());
			}
			return new RedirectToRouteResult("Login", new RouteValueDictionary());
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
                var ratesQuerable = db.Query<ExchangeRate>();
                var rates = db.Query<Currency>().ToList()
                    .Select(c => ExchangeRateProvider.Get(ratesQuerable, c.Id, d))
                    .Select(r => new ExchangeRateRest(r))
                    .ToList();
                return Json(rates, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
