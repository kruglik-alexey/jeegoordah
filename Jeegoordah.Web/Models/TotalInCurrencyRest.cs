using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class TotalInCurrencyRest
    {        
        public TotalInCurrencyRest(IList<BroTotalInCurrencyRest> totals, ExchangeRate rate)
        {
            Totals = totals;
            Rate = new ExchangeRateRest(rate);            
        }

        public IList<BroTotalInCurrencyRest> Totals { get; set; }
        public ExchangeRateRest Rate { get; set; }
    }
}