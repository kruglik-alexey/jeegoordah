using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class BroTotalRest
    {
        public class AmountRest
        {            
            public AmountRest(Currency currency, decimal amount)
            {
                Amount = amount;
                Currency = currency.Id;
            }

            public int Currency { get; set; }
            public decimal Amount { get; set; }
        }

        public BroTotalRest(Bro bro, Dictionary<Currency, decimal> amounts)
        {
            Bro = bro.Id;
            Amounts = amounts.Select(p => new AmountRest(p.Key, p.Value)).ToList();
        }

        public int Bro { get; set; }
        public IList<AmountRest> Amounts { get; set; }
    }
}