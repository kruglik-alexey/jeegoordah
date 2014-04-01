using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Jeegoordah.Droid.Repositories;
using System.Threading.Tasks;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.UI
{
    public class CreateTransactionFragment : Fragment
    {
		private readonly LocalRepository repository;
		private IList<Bro> bros;
		private IList<Currency> currencies;
		private IList<Event> events;
		private Spinner eventSelector;
		private Spinner currencySelector;
		private Spinner sourceSelector;
		private IList<Tuple<Bro, bool>> targets;

		public event Action TransactionCreated = () => {};

		public CreateTransactionFragment(LocalRepository repository)
		{
			this.repository = repository;
		}

		public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			await LoadEntities();
        }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var settings = Activity.GetSharedPreferences("jeegoordah.settings");
			var view = inflater.Inflate(Resource.Layout.CreateTransaction, container, false);

			eventSelector = view.FindViewById<Spinner>(Resource.Id.EventSelector);
			eventSelector.Adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, 
				events.OrderByDescending(e => e.GetRealStartDate()).Select(e => e.Name).ToArray());
			eventSelector.ItemSelected += (s, e) => targets = null;
			var defaultEvent = Helper.GetEntityFromSettings(events, settings, "defaultEvent");
			if (defaultEvent != null)
			{
				eventSelector.SetSelectedItem(defaultEvent.Name);
			}

			currencySelector = view.FindViewById<Spinner>(Resource.Id.CurrencySelector);
			currencySelector.Adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, 
				currencies.OrderBy(c => c.Name).Select(c => c.Name).ToArray());
			var defaultCurrency = Helper.GetEntityFromSettings(currencies, settings, "defaultCurrency");
			if (defaultCurrency != null)
			{
				currencySelector.SetSelectedItem(defaultCurrency.Name);
			}

			sourceSelector = view.FindViewById<Spinner>(Resource.Id.SourceSelector);
			sourceSelector.Adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerItem, 
				bros.OrderBy(b => b.Name).Select(b => b.Name).ToArray());

			view.FindViewById<Button>(Resource.Id.TargetsButton).Click += (s, e) => 
			{
				if (targets == null)
				{
					CreateDefaultTargets();
				}
				DialogFragment newFragment = new BroMultiSelector(targets, newTargets => targets = newTargets);
				newFragment.Show(FragmentManager, "");
			};		

			view.FindViewById<Button>(Resource.Id.CreateTransaction).Click += (s, e) => CreateTransaction();		

			return view;
		}

		private async Task LoadEntities()
		{
			events = await repository.GetEvents();
			bros = await repository.GetBros();
			currencies = await repository.GetCurencies();
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
				Amount = int.Parse(View.FindViewById<EditText>(Resource.Id.AmountInput).Text),
				Currency = GetSelectedCurrency().Id,
				Source = GetSelectedSource().Id,
				Targets = targets.Where(t => t.Item2).Select(t => t.Item1.Id).ToList(),
				Comment = View.FindViewById<EditText>(Resource.Id.CommentInput).Text
			};

			bool submitted = (await repository.PostTransaction(transaction)).HasValue;
			Toast.MakeText(Activity, "Transaction {0}".F(submitted ? "submitted" : "stored locally"), ToastLength.Short).Show();
			TransactionCreated();
			//UpdatePendingTransactionCount();
			Clear();
		}

		private void Clear()
		{
			View.FindViewById<EditText>(Resource.Id.AmountInput).Text = "";
			View.FindViewById<EditText>(Resource.Id.CommentInput).Text = "";
			targets = null;
		}

		private bool Validate()
		{
			if (View.FindViewById<EditText>(Resource.Id.AmountInput).Text.Trim() == "")
			{
				Toast.MakeText(Activity, "Please enter amount", ToastLength.Short).Show();
				return false;
			}
			return true;
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
    }
}