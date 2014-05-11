using System;
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

        async public static Task Update(Db db)
        {
            await new ExchangeRatesUpdater(db).Update();
        }

        private ExchangeRatesUpdater(Db db)
        {
            this.db = db;
        }

        async private Task Update()
        {
            IList<Currency> currencies = db.Query<Currency>().ToList();
            // TODO do not update base currency?
            foreach (Currency currency in currencies)
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
            decimal rate = await GetRate(currency, date);
            logger.I("Rate for {0} on {1} is {2}", currency.Name, date.ToShortDateString(), rate);
            db.Session.Save(new ExchangeRate {Currency = currency, Date = date, Rate = rate});
        }

        async private Task<decimal> GetRate(Currency currency, DateTime date)
        {
            JToken ratesForDate;
            if (!rates.TryGetValue(date, out ratesForDate))
            {
                ratesForDate = await LoadRatesForDate(date);
                rates[date] = ratesForDate;
            }
            return ratesForDate[currency.Name].ToObject<decimal>();
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
