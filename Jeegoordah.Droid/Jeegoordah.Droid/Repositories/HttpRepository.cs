using System;
using System.Net.Http;
using Android.Util;
using Java.Lang;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeegoordah.Droid.Entities;
using System.Text;
using Exception = System.Exception;
using System.Net;

namespace Jeegoordah.Droid.Repositories
{
	public class HttpRepository : IHttpRepository
    {
		private readonly string host;
        private readonly int? port = null;
        private readonly HttpClient _httpClient;

		public HttpRepository()
        {
#if DEBUG
			host = "jeegoordah-test.azurewebsites.net";
#else
			host ="jeegoordah.azurewebsites.net";
#endif

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Cookie("FB83D6D9-6D18-419E-B862-1A1DBE30F536", "1"){ Domain = host });
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            _httpClient = new HttpClient(handler);
        }

		public async Task<IList<Bro>> GetBros()
		{
			return await Get<IList<Bro>>("bros");
		}

		public async Task<IList<Currency>> GetCurencies()
		{
			return await Get<IList<Currency>>("currencies");
		}

		public async Task<IList<Event>> GetEvents()
		{
			return await Get<IList<Event>>("events");
		}

        public async Task<CurrencyTotal> GetTotal(int currencyId)
		{
            return await Get<CurrencyTotal>("total/{0}".F(currencyId));
		}

	    public async Task<IList<ExchangeRate>> GetRates(DateTime date)
	    {
            return await Get<IList<ExchangeRate>>("rates/{0}".F(date.ToJson()));
	    }

	    public async Task<int?> PostTransaction(Transaction transaction)
		{
			return (await Post(transaction, "transactions/create")).Id;
		}

		private async Task<T> Get<T>(string resourceName)
		{			
		    var url = GetUrl(resourceName);
            Logger.Info(this, "GET {0}", url);
            var content = await _httpClient.GetStringAsync(url);
			return JsonConvert.DeserializeObject<T>(content);			
        }         

    	private async Task<T> Post<T>(T data, string resourceName)
    	{			
			var content = JsonConvert.SerializeObject(data);
			var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = GetUrl(resourceName);
            Logger.Info(this, "POST {0}", url);
            using (var response = await _httpClient.PostAsync(url, httpContent))
			{
			    if (!response.IsSuccessStatusCode)
			        throw new Exception("Server Error. Status code: {0}".F(response.StatusCode));
			    var r = await response.Content.ReadAsStringAsync();
			    return JsonConvert.DeserializeObject<T>(r);
			}			
    	}

		private string GetUrl(string resourceName)
		{
		    return port.HasValue
		        ? "http://{0}:{1}/{2}".F(host, port.Value, resourceName)
		        : "http://{0}/{1}".F(host, resourceName);
		}
    }
}

