using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.Repositories
{
	public interface IHttpRepository
	{
		Task<IList<Bro>> GetBros();
		Task<IList<Currency>> GetCurencies();
		Task<IList<Event>> GetEvents();
        Task<CurrencyTotal> GetTotal(int currencyId);
		Task<IList<ExchangeRate>> GetRates(DateTime date);
		Task<int?> PostTransaction(Transaction transaction);
	}	
}
