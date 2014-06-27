using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class ExchangeRateRest
    {
        public ExchangeRateRest(ExchangeRate rate)
        {
            Currency = rate.Currency.Id;
            Date = JsonDate.ToString(rate.Date);
            Rate = rate.Rate;
        }

        public int Currency { get; set; }
        public string Date { get; set; }
        public decimal Rate { get; set; }        
    }
}