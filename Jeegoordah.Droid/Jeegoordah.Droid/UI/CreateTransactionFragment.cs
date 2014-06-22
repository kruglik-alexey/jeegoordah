using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Text;
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
        private EditText amountInput;
        private EditText rateInput;
        private EditText amountInBaseInput;
		private IList<Tuple<Bro, bool>> targets;
        private IList<ExchangeRate> rates;

        public event Action TransactionCreated = () => {};

		public CreateTransactionFragment(LocalRepository repository)
		{
			this.repository = repository;
		}

		public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			LoadEntities();
        }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var settings = Activity.GetSharedPreferences("jeegoordah.settings");
			var view = inflater.Inflate(Resource.Layout.CreateTransaction, container, false);

            amountInput = view.FindViewById<EditText>(Resource.Id.AmountInput);              
            rateInput = view.FindViewById<EditText>(Resource.Id.RateInput);
            amountInBaseInput = view.FindViewById<EditText>(Resource.Id.AmountInBaseInput);

            amountInput.TextChanged += (_, __) => UpdateAmountInBase();
            rateInput.TextChanged += (_, __) => UpdateAmountInBase();

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
            currencySelector.ItemSelected += (_, __) => CurrencyChanged();
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

        private void CurrencyChanged()
        {
            var currency = GetSelectedCurrency();
            var rate = rates.FirstOrDefault(r => r.Currency == currency.Id);
            rateInput.Text = rate != null ? rate.Rate.ToString("", Thread.CurrentThread.CurrentCulture) : "";
        }       

        private void UpdateAmountInBase()
        {
            amountInBaseInput.Text = "";
            decimal rate;
            decimal amount;
            if (!(decimal.TryParse(rateInput.Text, out rate) && decimal.TryParse(amountInput.Text, out amount)))
                return;
            amountInBaseInput.Text = (amount/rate).ToString("N2", Thread.CurrentThread.CurrentCulture);
        }

        private void LoadEntities()
		{
			events = repository.GetEvents();
			bros = repository.GetBros();
			currencies = repository.GetCurencies();
            rates = repository.GetRates();
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
				Amount = int.Parse(amountInput.Text),
                Rate = decimal.Parse(rateInput.Text).ToString(CultureInfo.InvariantCulture),
				Currency = GetSelectedCurrency().Id,
				Source = GetSelectedSource().Id,
				Targets = targets.Where(t => t.Item2).Select(t => t.Item1.Id).ToList(),
				Comment = View.FindViewById<EditText>(Resource.Id.CommentInput).Text
			};
		    
            PostTransactionResult result = await repository.PostTransaction(transaction);
		    if (result.StoredLocally)
		    {
                Toast.MakeText(Activity, "Transaction stored locally: {0}".F(result.ReasonStoredLocally), ToastLength.Long).Show();
		    }
		    else
		    {
                Toast.MakeText(Activity, "Transaction stored remotely", ToastLength.Long).Show();
		    }
			
			TransactionCreated();			
			Clear();
		}

		private void Clear()
		{
			amountInput.Text = "";		    
			View.FindViewById<EditText>(Resource.Id.CommentInput).Text = "";
			targets = null;
		}

		private bool Validate()
		{
			if (amountInput.Text.Trim() == "")
			{
				Toast.MakeText(Activity, "Please enter amount", ToastLength.Short).Show();
			    amountInput.RequestFocus();
				return false;
			}
            if (rateInput.Text.Trim() == "")
            {
                Toast.MakeText(Activity, "Please enter rate", ToastLength.Short).Show();
                rateInput.RequestFocus();
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