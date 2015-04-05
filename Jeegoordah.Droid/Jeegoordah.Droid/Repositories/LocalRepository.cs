using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.Repositories
{
    public class PostTransactionResult
    {
        public bool StoredLocally { get; private set; }
        public string ReasonStoredLocally { get; private set; }

        public PostTransactionResult(bool storedLocally, string reasonStoredLocally)
        {
            if (!storedLocally && reasonStoredLocally != null) throw new ArgumentException();
            StoredLocally = storedLocally;
            ReasonStoredLocally = reasonStoredLocally;
        }
    }

	public class LocalRepository
	{
		private const string brosKey = "bros";
		private const string currenciesKey = "currencies";
		private const string eventsKey = "events";
		private const string totalKey = "total";		
		private const string ratesKey = "rates";

        private readonly IHttpRepository parentRepository;
		private readonly ISharedPreferences localCache;
		private readonly ISharedPreferences pendingTransactions;
		private readonly ISharedPreferences miscStorage;
		private readonly IConnectionHelper connectionHelper;	

		public LocalRepository(IHttpRepository parentRepository, IConnectionHelper connectionHelper, ISharedPreferencesProvider sharedPreferencesProvider)
		{
			this.connectionHelper = connectionHelper;
			this.localCache = sharedPreferencesProvider.Get("jeegoordah.localCache");
			this.pendingTransactions = sharedPreferencesProvider.Get("jeegoordah.pendingTransactions");
			this.miscStorage = sharedPreferencesProvider.Get("jeegoordah.misc");
			this.parentRepository = parentRepository;			
		}	

		public IList<Bro> GetBros()
		{
			return Get<IList<Bro>>(brosKey);
		}

		public IList<Currency> GetCurencies()
		{
			return Get<IList<Currency>>(currenciesKey);
		}

		public IList<Event> GetEvents()
		{
			return Get<IList<Event>>(eventsKey);
		}

        public CurrencyTotal GetTotal()
		{
            return Get<CurrencyTotal>(totalKey);
		}

	    public IList<ExchangeRate> GetRates()
	    {
	        return Get<IList<ExchangeRate>>(ratesKey);
	    }

        public async Task<PostTransactionResult> PostTransaction(Transaction transaction)
		{
	        if (!connectionHelper.HasConnection)
	        {
	            AddPendingTransaction(transaction);
	            return new PostTransactionResult(true, "No connection");
	        }

            int? id = null;
	        try
	        {
	            id = await parentRepository.PostTransaction(transaction);                	
                await Put(totalKey, parentRepository.GetTotal(GetBaseCurrency().Id));
	            return new PostTransactionResult(false, null);
	        }
	        catch (Exception ex)
	        {
	            if (id.HasValue) throw;
	            AddPendingTransaction(transaction);
	            return new PostTransactionResult(true, ex.Message);
	        }            
		}

        public Currency GetBaseCurrency()
        {
            return GetCurencies().Single(x => x.IsBase);
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

		    var now = DateTime.UtcNow;
            Logger.Info(this, "Refreshing");

            var submitPendingTask = SubmitPendingTransactions();
            var currenciesTask = Put(currenciesKey, parentRepository.GetCurencies());

            await Task.WhenAll(new Task[]
            {
                Task.WhenAll(new Task[]{ submitPendingTask, currenciesTask }).ContinueWith(async _ =>
                {
                    await Put(totalKey, parentRepository.GetTotal(GetBaseCurrency().Id));
                }),
                Put(brosKey, parentRepository.GetBros()),
                Put(eventsKey, parentRepository.GetEvents()),				
                Put(ratesKey, parentRepository.GetRates(now)),			
            });          

			using (var e = miscStorage.Edit())
			{
                e.PutLong("lastRefresh", now.Ticks);
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
				return lastRefresh != 0 ? new DateTime(lastRefresh).ToLocalTime() : (DateTime?)null;
			}
		}	

		private async Task<T> Put<T>(string key, Task<T> dataSource)
		{
            Logger.Info(this, "Retrieving data for " + key);
			T data = await dataSource;
            Logger.Info(this, "Received data for " + key);
			string serialized = JsonConvert.SerializeObject(data);
			using (var editor = localCache.Edit())
			{				
				editor.PutString(key, serialized);
				editor.Commit();
			}
		    return data;
		}

		private T Get<T>(string key)
		{
			return JsonConvert.DeserializeObject<T>(localCache.GetString(key, ""));			
		}	

		private async Task SubmitPendingTransactions()
		{
			using (var e = pendingTransactions.Edit())
			{
				try
				{
                    var list = pendingTransactions.All.ToList();
                    Logger.Info(this, "Submitting {0} pending transactions", list.Count);
                    foreach (KeyValuePair<string, object> pair in list)
					{
						var transaction = JsonConvert.DeserializeObject<Transaction>(pair.Value.ToString());
						await parentRepository.PostTransaction(transaction);
						e.Remove(pair.Key);
					}
                    Logger.Info(this, "Submitted pending transactions");
				}
				finally
				{					
					e.Commit();
				}
			}
		}        
	}	
}
