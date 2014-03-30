using System;
using Android.Content;
using Android.Net;
using System.Linq;

namespace Jeegoordah.Droid
{
	public class ActivityRepositoryAdapter : IConnectionHelper, ISharedPreferencesProvider
    {
		private readonly Context context;

		public ActivityRepositoryAdapter(Context context)
        {
			this.context = context;
        }

		public bool HasConnection
		{
			get
			{
				return ((ConnectivityManager)context.GetSystemService(Context.ConnectivityService)).GetAllNetworkInfo().Any(c => c.IsConnected);
			}
		}	

		public ISharedPreferences Get(string name)
		{
			return context.GetSharedPreferences(name);
		}	
    }
}

