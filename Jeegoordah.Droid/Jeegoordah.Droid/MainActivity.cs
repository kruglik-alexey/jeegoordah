using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Jeegoordah.Droid.Entities;
using Jeegoordah.Droid.Repositories;
using Android.Net;

namespace Jeegoordah.Droid
{
    [Activity(Label = "Jeegoordah.Droid", MainLauncher = true)]
	public class MainActivity : Activity, IConnectionHelper, ISharedPreferencesProvider
    {
		private LocalRepository repo;
		private IList<Bro> bros;
		private IList<Currency> currencies;
		private IList<Event> events;
		private Spinner eventSelector;
		private Spinner currencySelector;
		private Spinner sourceSelector;
		private IList<Tuple<Bro, bool>> targets;

		protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			CreateRepository();
			await TryUpdateFromWeb();
			await LoadEntities();

			var lastRefresh = repo.LastRefresh;
			if (!lastRefresh.HasValue)
			{
				return;
			}

			SetContentView(Resource.Layout.Main);
			FindViewById<TextView>(Resource.Id.LastRefresh).Text = "Last sync: {0}".F(lastRefresh);

			eventSelector = FindViewById<Spinner>(Resource.Id.EventSelector);
			eventSelector.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, 
				events.OrderByDescending(e => e.GetRealStartDate()).Select(e => e.Name).ToArray());
			eventSelector.ItemSelected += (s, e) => targets = null;

			currencySelector = FindViewById<Spinner>(Resource.Id.CurrencySelector);
			currencySelector.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, 
				currencies.OrderBy(c => c.Name).Select(c => c.Name).ToArray());

			sourceSelector = FindViewById<Spinner>(Resource.Id.SourceSelector);
			sourceSelector.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, 
				bros.OrderBy(b => b.Name).Select(b => b.Name).ToArray());

			FindViewById<Button>(Resource.Id.TargetsButton).Click += (s, e) => 
			{
				if (targets == null)
				{
					CreateDefaultTargets();
				}
				DialogFragment newFragment = new BroMultiSelector(targets, newTargets => targets = newTargets);
				newFragment.Show(FragmentManager, "missiles");
			};		

			FindViewById<Button>(Resource.Id.CreateTransaction).Click += (s, e) => CreateTransaction();		
			UpdatePendingTransactionCount();
        }

		private void CreateDefaultTargets()
		{
			var selectedEvent = GetSelectedEvent();
			targets = bros.Select(b => Tuple.Create(b, selectedEvent.Bros.Contains(b.Id))).ToList();
		}

		private async void CreateTransaction()
		{
			if (targets == null)
			{
				CreateDefaultTargets();
			}

            if (!Validate())
            {
                return;
            }

			var transaction = new Transaction
			{
				Date = DateTime.UtcNow.ToString("dd-MM-yyyy"),
				Event = GetSelectedEvent().Id,
				Amount = int.Parse(FindViewById<EditText>(Resource.Id.AmountInput).Text),
				Currency = GetSelectedCurrency().Id,
				Source = GetSelectedSource().Id,
				Targets = targets.Where(t => t.Item2).Select(t => t.Item1.Id).ToList(),
				Comment = FindViewById<EditText>(Resource.Id.CommentInput).Text
			};

			bool submitted = (await repo.PostTransaction(transaction)).HasValue;
			Toast.MakeText(this, "Transaction {0}".F(submitted ? "submitted" : "stored locally"), ToastLength.Short).Show();			
			UpdatePendingTransactionCount();
            Clear();
		}

        private void Clear()
        {
            FindViewById<EditText>(Resource.Id.AmountInput).Text = "";
            FindViewById<EditText>(Resource.Id.CommentInput).Text = "";
            targets = null;
        }

        private bool Validate()
        {
            if (FindViewById<EditText>(Resource.Id.AmountInput).Text.Trim() == "")
            {
                Toast.MakeText(this, "Please enter amount", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

		private void UpdatePendingTransactionCount()
		{
			FindViewById<TextView>(Resource.Id.PendingTransactionCount).Text =  "Pending transactions: {0}".F(repo.PendingTransactionCount.ToString());
		}

		private Event GetSelectedEvent()
		{
			return events.First(e => e.Name == eventSelector.SelectedItem.ToString());
		}

		private Currency GetSelectedCurrency()
		{
			return currencies.First(c => c.Name == currencySelector.SelectedItem.ToString());
		}

		private Bro GetSelectedSource()
		{
			return bros.First(b => b.Name == sourceSelector.SelectedItem.ToString());
		}	
	
		private void CreateRepository()
		{
			string host;
#if DEBUG
			host = "jeegoordah-test.azurewebsites.net";
#else
			host ="jeegoordah.azurewebsites.net";
#endif
			repo = new LocalRepository(new HttpRepository(host), this, this);
		}

		private async Task TryUpdateFromWeb()
		{
			var progress = ProgressDialog.Show(this, "Please wait", "Loading data from Jeegoordah", true);
			try
			{
				if (await repo.Refresh())
					Toast.MakeText(this, "Synchronized with Jeegoordah", ToastLength.Short).Show();
			} 
			catch (Exception ex)
			{
				Toast.MakeText(this, "Fail: {0}".F(ex.Message), ToastLength.Long).Show();
			}
			finally
			{
				progress.Dismiss();	
			}
		}

		private async Task LoadEntities()
		{
			events = await repo.GetEvents();
			bros = await repo.GetBros();
			currencies = await repo.GetCurencies();
		}
	
		bool IConnectionHelper.HasConnection
		{
			get
			{
                return ((ConnectivityManager)GetSystemService(Context.ConnectivityService)).GetAllNetworkInfo().Any(n => n.IsConnected);
			}
		}
	
		ISharedPreferences ISharedPreferencesProvider.Get(string name)
		{
            return GetSharedPreferences(name, FileCreationMode.Private);
		}
    }
}


