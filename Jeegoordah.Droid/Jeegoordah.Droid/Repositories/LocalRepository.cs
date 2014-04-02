using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Newtonsoft.Json;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.Repositories
{
	public class LocalRepository : IRepository
	{
		private const string brosKey = "bros";
		private const string currenciesKey = "currencies";
		private const string eventsKey = "events";
		private const string totalKey = "total";
		private const string lastRefreshKey = "lastRefresh";

		private readonly IRepository parentRepository;
		private readonly ISharedPreferences localCache;
		private readonly ISharedPreferences pendingTransactions;
		private readonly ISharedPreferences miscStorage;
		private readonly IConnectionHelper connectionHelper;	

		public LocalRepository(IRepository parentRepository, IConnectionHelper connectionHelper, ISharedPreferencesProvider sharedPreferencesProvider)
		{
			this.connectionHelper = connectionHelper;
			this.localCache = sharedPreferencesProvider.Get("jeegoordah.localCache");
			this.pendingTransactions = sharedPreferencesProvider.Get("jeegoordah.pendingTransactions");
			this.miscStorage = sharedPreferencesProvider.Get("jeegoordah.misc");
			this.parentRepository = parentRepository;			
		}	

		public Task<IList<Bro>> GetBros()
		{
			return Get<IList<Bro>>(brosKey);
		}

		public Task<IList<Currency>> GetCurencies()
		{
			return Get<IList<Currency>>(currenciesKey);
		}

		public Task<IList<Event>> GetEvents()
		{
			return Get<IList<Event>>(eventsKey);
		}

		public Task<IList<BroTotal>> GetTotal()
		{
			return Get<IList<BroTotal>>(totalKey);
		}

		public async Task<int?> PostTransaction(Transaction transaction)
		{
			if (connectionHelper.HasConnection)
			{
				int? id = null;
				try
				{
					id = await parentRepository.PostTransaction(transaction);                	
					await Put(totalKey, parentRepository.GetTotal());
					return id;
				}
				catch (Exception)
				{
					if (!id.HasValue)
						AddPendingTransaction(transaction);
					throw;
				}
			}

			AddPendingTransaction(transaction);
			return null;
		}

		private void AddPendingTransaction(Transaction transaction)
		{
			using (var e = pendingTransactions.Edit())
			{
				e.PutString(Guid.NewGuid().ToString(), JsonConvert.SerializeObject(transaction));
				e.Commit();
			}
		}

		public async Task<bool> Refresh()
		{
			if (!connectionHelper.HasConnection)
			{
				return false;
			}

			await Task.WhenAll(new []
			{
				Put(brosKey, parentRepository.GetBros()),
				Put(eventsKey, parentRepository.GetEvents()),
				Put(currenciesKey, parentRepository.GetCurencies()),				
				SubmitPendingTransactions()
			});
			await Put(totalKey, parentRepository.GetTotal());

			using (var e = miscStorage.Edit())
			{
				e.PutLong("lastRefresh", DateTime.UtcNow.Ticks);
				e.Commit();
			}
			return true;
		}

		public IList<Transaction> GetPendingTransactions()
		{
			return pendingTransactions.All.Select(p => JsonConvert.DeserializeObject<Transaction>(p.Value.ToString())).ToList();
		}

		public DateTime? LastRefresh
		{
			get 
			{
				long lastRefresh = miscStorage.GetLong("lastRefresh", 0);
				return lastRefresh != 0 ? DateTime.FromFileTime(lastRefresh).ToLocalTime() : (DateTime?)null;
			}
		}	

		private async Task Put<T>(string key, Task<T> dataSource)
		{
			T data = await dataSource;
			string serialized = JsonConvert.SerializeObject(data);
			using (var editor = localCache.Edit())
			{				
				editor.PutString(key, serialized);
				editor.Commit();
			}
		}

		private Task<T> Get<T>(string key)
		{
			T data = JsonConvert.DeserializeObject<T>(localCache.GetString(key, ""));
			return Task.FromResult(data);
		}	

		private async Task SubmitPendingTransactions()
		{
			using (var e = pendingTransactions.Edit())
			{
				try
				{
					foreach (KeyValuePair<string, object> pair in pendingTransactions.All.ToList())
					{
						var transaction = JsonConvert.DeserializeObject<Transaction>(pair.Value.ToString());
						await parentRepository.PostTransaction(transaction);
						e.Remove(pair.Key);
					}	
				}
				finally
				{					
					e.Commit();
				}
			}
		}
	}	
}
