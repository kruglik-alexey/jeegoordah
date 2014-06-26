using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class TotalInCurrencyRest
    {        
        public TotalInCurrencyRest(IList<BroTotalInCurrencyRest> totals, Currency currency, ExchangeRate rate)
        {
            Totals = totals;
            Currency = currency.Id;
            Rate = rate.Rate;
        }

        public IList<BroTotalInCurrencyRest> Totals { get; set; }
        public int Currency { get; set; }
        public decimal Rate { get; set; }
    }
}