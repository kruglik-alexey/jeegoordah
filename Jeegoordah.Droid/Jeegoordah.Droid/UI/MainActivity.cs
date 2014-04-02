using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Jeegoordah.Droid.Repositories;
using Jeegoordah.Droid.UI;

namespace Jeegoordah.Droid.UI
{
	[Activity(Label = "JGDH", MainLauncher = true)]
	public class MainActivity : Activity, ActionBar.IOnNavigationListener
    {
		private LocalRepository repository;
		private int cirrentNavigation = -1;
		private Fragment activeFragment;

		protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			var repositoryAdapter = new ActivityRepositoryAdapter(this);
			repository = new LocalRepository(new HttpRepository(), repositoryAdapter, repositoryAdapter);
			await TryUpdateFromWeb();

			var lastRefresh = repository.LastRefresh;
			if (!lastRefresh.HasValue)
			{
				Toast.MakeText(this, "Need connection for the first run", ToastLength.Long).Show();
				return;
			}

			SetContentView(Resource.Layout.Main);
			SetNavigation();
			FindViewById<TextView>(Resource.Id.LastRefresh).Text = "Last sync: {0}".F(lastRefresh);
			UpdatePendingTransactionCount();
        }

		private void SetNavigation()
		{
			ActionBar.NavigationMode = ActionBarNavigationMode.List;
			ArrayAdapter<string> items = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, new[] {"Transactions", "Total"});
			ActionBar.SetListNavigationCallbacks(items, this);
		}		 

		private void UpdatePendingTransactionCount()
		{
			FindViewById<TextView>(Resource.Id.PendingTransactionCount).Text = "Pending transactions: {0}".F(repository.GetPendingTransactions().Count.ToString());
		}

		private async Task TryUpdateFromWeb()
		{
			var progress = ProgressDialog.Show(this, "Please wait", "Loading data from Jeegoordah", true);
			try
			{
				if (await repository.Refresh())
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

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.SettingsMenu:
				{
					var intent = new Intent(this, typeof(SettingsActivity));
					StartActivity(intent);
					break;
				}
				default: throw new ArgumentOutOfRangeException();
			}
			return true;
		}

		public bool OnNavigationItemSelected(int itemPosition, long itemId)
		{
			if (cirrentNavigation == itemPosition)
				return true;

			activeFragment.As<CreateTransactionFragment>(f => f.TransactionCreated -= UpdatePendingTransactionCount);

			switch (itemPosition)
			{
				case 0:
				{
					var createTransactionFragment = new CreateTransactionFragment(repository);
					createTransactionFragment.TransactionCreated += UpdatePendingTransactionCount;
					activeFragment = createTransactionFragment;
					break;
				}
				case 1:
				{
					activeFragment = new TotalFragment(repository);
					break;
				}
				default: throw new ArgumentOutOfRangeException();
			}

			FindViewById<ViewGroup>(Resource.Id.Content).RemoveAllViews();
			using (var t = FragmentManager.BeginTransaction())
			{
				t.Add(Resource.Id.Content, activeFragment);
				t.Commit();
			}
			cirrentNavigation = itemPosition;
			return true;
		}
    }
}