using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static Dictionary<Bro, decimal> CalculateInBaseCurrency(IList<Transaction> transactions, IList<Bro> bros)
        {
            var result = new Dictionary<Bro, decimal>(bros.Count);
            foreach (Bro bro in bros)
            {
                result[bro] = 0;
            }

            foreach (Transaction t in transactions)
            {
                decimal amount = t.Amount/t.Rate;
                decimal share = amount/t.Targets.Count;
                foreach (Bro bro in t.Targets)
                {
                    result[bro] -= share;
                }
                result[t.Source] += amount;   
            }
            return result;
        }

        public static Dictionary<Bro, decimal> CalculateInCurrency(ExchangeRate rate, IList<Transaction> transactions, IList<Bro> bros)
        {            
            return CalculateInBaseCurrency(transactions, bros).ToDictionary(p => p.Key, p => p.Value * rate.Rate);
        }
    }
}
