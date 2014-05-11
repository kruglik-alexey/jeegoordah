using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class BroTotalInBaseCurrencyRest
    {
        public BroTotalInBaseCurrencyRest(Bro bro, decimal amount)
        {
            Bro = bro.Id;
            Amount = amount;            
        }

        public int Bro { get; set; }
        public decimal Amount { get; set; }
    }
}