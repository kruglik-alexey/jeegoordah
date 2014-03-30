using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeegoordah.Droid.Entities;
using System.Text;

namespace Jeegoordah.Droid.Repositories
{
	public class HttpRepository : IRepository
    {
		private readonly string host;

		public HttpRepository()
        {
#if DEBUG
			host = "jeegoordah-test.azurewebsites.net";
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

		public async Task<int?> PostTransaction(Transaction transaction)
		{
			return (await Post(transaction, "transactions/create")).Id;
		}

		private async Task<T> Get<T>(string resourceName)
		{
			using (var client = new HttpClient())
			{
				var content = await client.GetStringAsync(GetUrl(resourceName));
				return JsonConvert.DeserializeObject<T>(content);
			}
		}	

		private async Task<T> Post<T>(T data, string resourceName)
		{
			using (var client = new HttpClient())
			{
				var content = JsonConvert.SerializeObject(data);
				var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
				using (var response = await client.PostAsync(GetUrl(resourceName), httpContent))
				{
					var r = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<T>(r);
				}
			}
		}

		private string GetUrl(string resourceName)
		{
			return "http://{0}/{1}".F(host, resourceName);
		}
    }
}

