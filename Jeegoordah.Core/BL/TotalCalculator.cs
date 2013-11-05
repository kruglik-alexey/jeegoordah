using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Core.BL
{
    public static class TotalCalculator
    {
        public static Dictionary<Bro, Dictionary<Currency, decimal>> Calculate(IList<Transaction> transactions, IList<Bro> bros)
        {            
            var currencies = transactions.Select(t => t.Currency).Distinct().ToList();
            var result = new Dictionary<Bro, Dictionary<Currency, decimal>>(bros.Count);
            foreach (Bro bro in bros)
            {
                result[bro] = new Dictionary<Currency, decimal>(currencies.Count);
                foreach (Currency currency in currencies)
                {
                    result[bro][currency] = 0;
                }
            }

            foreach (Transaction t in transactions)
            {
                decimal share = t.Amount / t.Targets.Count;
                foreach (Bro bro in t.Targets)
                {
                    result[bro][t.Currency] -= share;
                }
                result[t.Source][t.Currency] += t.Amount;                
            }

            return result;
        }
    }
}
