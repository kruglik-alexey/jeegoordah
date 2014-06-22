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

namespace Jeegoordah.Droid.Repositories
{
	public class HttpRepository : IHttpRepository
    {
		private readonly string host;
	    private readonly int? port;

		public HttpRepository()
        {
#if DEBUG
			host = "jeegoordah-test.azurewebsites.net";
		    host = "192.168.1.2";
		    port = 3107;
#else
			host ="jeegoordah.azurewebsites.net";
#endif
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

		public async Task<IList<BroTotal>> GetTotal()
		{
			return await Get<IList<BroTotal>>("total");
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
			using (var client = new HttpClient())
			{
			    var url = GetUrl(resourceName);
			    Log.Info(GetType().Name, "GET {0}".F(url));
				var content = await client.GetStringAsync(url);
				return JsonConvert.DeserializeObject<T>(content);
			}
		}	

		private async Task<T> Post<T>(T data, string resourceName)
		{
			using (var client = new HttpClient())
			{
				var content = JsonConvert.SerializeObject(data);
				var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                var url = GetUrl(resourceName);
                Log.Info(GetType().Name, "POST {0}".F(url));
                using (var response = await client.PostAsync(url, httpContent))
				{
				    if (!response.IsSuccessStatusCode)
				        throw new Exception("Server Error. Status code: {0}".F(response.StatusCode));
				    var r = await response.Content.ReadAsStringAsync();
				    return JsonConvert.DeserializeObject<T>(r);
				}
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

