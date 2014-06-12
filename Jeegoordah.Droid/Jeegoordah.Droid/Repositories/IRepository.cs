using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.Repositories
{
	public interface IRepository
	{
		Task<IList<Bro>> GetBros();
		Task<IList<Currency>> GetCurencies();
		Task<IList<Event>> GetEvents();
		Task<IList<BroTotal>> GetTotal();
		Task<IList<ExchangeRate>> GetRates(DateTime date);
		Task<int?> PostTransaction(Transaction transaction);
	}	
}
