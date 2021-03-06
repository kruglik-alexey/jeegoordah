﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Jeegoordah.Core.DL; 
using Jeegoordah.Core.DL.Entity;
using Jeegoordah.Core.Logging;
using Newtonsoft.Json.Linq;

namespace Jeegoordah.Core.BL
{
    public class ExchangeRatesUpdater
    {
        private const string RatesProviderUrl = "https://openexchangerates.org/api/historical/{0}.json?app_id=8fefe9f6d2054e61a970d1fabdb628bb";
        private readonly Db db;
        private readonly Dictionary<DateTime, JToken> rates = new Dictionary<DateTime, JToken>();
        private readonly Logger logger = Logger.For(typeof(ExchangeRatesUpdater));
	    private readonly Currency byn;

        async public static Task Update(Db db)
        {
            await new ExchangeRatesUpdater(db).Update();
        }

        private ExchangeRatesUpdater(Db db)
        {
            this.db = db;
	        byn = db.Query<Currency>().Where(c => c.Name == "BYN").First();
        }

        async private Task Update()
        {
            IList<Currency> currencies = db.Query<Currency>().ToList();            
            foreach (Currency currency in currencies.Where(c => c.Name != "CUC" && c.Name != "BYN"))
            {
                await UpdateCurrencyRates(currency);
            }
        }

	    async private Task UpdateCurrencyRates(Currency currency)
        {
            var lastRate = db.Query<ExchangeRate>()
                .Where(r => r.Currency.Id == currency.Id)
                .OrderByDescending(r => r.Date)
                .FirstOrDefault();
            var today = DateTime.UtcNow.Date;
            var date = lastRate != null ? lastRate.Date.AddDays(1) : DateTime.UtcNow.Date;            
            while (date <= today)
            {
                await UpdateCurrencyRate(currency, date);
                date = date.AddDays(1);
            }
        }

        async private Task UpdateCurrencyRate(Currency currency, DateTime date)
        {
            decimal? rate = await GetRate(currency, date);
            if (rate.HasValue)
            {
                logger.I("Rate for {0} on {1} is {2}", currency.Name, date.ToShortDateString(), rate);
                db.Session.Save(new ExchangeRate {Currency = currency, Date = date, Rate = Math.Round(rate.Value, 2)});
	            if (currency.Name == "BYR")
	            {
					logger.I("Rate for {0} on {1} is {2}", byn.Name, date.ToShortDateString(), rate.Value/10000);
					db.Session.Save(new ExchangeRate { Currency = byn, Date = date, Rate = Math.Round(rate.Value/10000, 2) });
				}
            }
            else
            {
                logger.I("There is no rate for {0} on {1}", currency.Name, date.ToShortDateString());
            }
        }

        async private Task<decimal?> GetRate(Currency currency, DateTime date)
        {
            JToken ratesForDate;
            if (!rates.TryGetValue(date, out ratesForDate))
            {
                ratesForDate = await LoadRatesForDate(date);
                rates[date] = ratesForDate;
            }
            var rate = ratesForDate[currency.Name];
            return rate != null ? rate.ToObject<decimal>() : (decimal?)null;
        }

        async private Task<JToken> LoadRatesForDate(DateTime dateTime)
        {
            logger.I("Downloading rates on {0}", dateTime.ToShortDateString());        
            var response = await new HttpClient().GetStringAsync(RatesProviderUrl.F(dateTime.ToString("yyyy-MM-dd")));
            JObject parsedResponse = JObject.Parse(response);
            return parsedResponse["rates"];
        }        
    }
}
