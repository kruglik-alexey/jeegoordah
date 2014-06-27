using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Core.BL
{
    public static class ExchangeRateProvider
    {
        public static ExchangeRate Get(IQueryable<ExchangeRate> source, int currencyId, DateTime date)
        {
            return source.Where(r => r.Currency.Id == currencyId)
                .Where(r => r.Date <= date)
                .OrderByDescending(r => r.Date)
                .First();
        }
    }
}
