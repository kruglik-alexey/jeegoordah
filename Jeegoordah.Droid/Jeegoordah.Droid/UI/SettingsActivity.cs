using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Jeegoordah.Droid.Repositories;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.UI
{
    [Activity(Label = "Settings")]			
	public class SettingsActivity : Activity
    {
		private ISharedPreferences settings;
		private LocalRepository repo;
		private IList<Currency> currencies;
		private IList<Event> events;

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Settings);
			settings = this.GetSharedPreferences("jeegoordah.settings");
			var repositoryAdapter = new ActivityRepositoryAdapter(this);
			repo = new LocalRepository(new HttpRepository(), repositoryAdapter, repositoryAdapter);

			currencies = repo.GetCurencies();
			events = repo.GetEvents();

			SetupEvents();
			SetupCurrencies();
        }

		private void SetupCurrencies()
		{
			var currencySelector = FindViewById<Spinner>(Resource.Id.CurrencySelector);
			var currencysAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, 
				currencies.OrderByDescending(c => c.Name).Select(c => c.Name).ToArray());
			currencySelector.Adapter = currencysAdapter;

			var defaultCurrency = Helper.GetEntityFromSettings(currencies, settings, "defaultCurrency");			
			if (defaultCurrency != null)
			{
				currencySelector.SetSelectedItem(defaultCurrency.Name);
			}

			currencySelector.ItemSelected += (_, __) =>
			{
				using (var edit = settings.Edit())
				{
					var selectedCurrency = currencySelector.SelectedItem.ToString();
					edit.PutInt("defaultCurrency", currencies.First(e => e.Name == selectedCurrency).Id);
					edit.Commit();
				}
			};
		}

		private void SetupEvents()
		{
			var eventSelector = FindViewById<Spinner>(Resource.Id.EventSelector);
            var eventNames = new[]{ Consts.NoEvent }.Concat(events.OrderByDescending(e => e.GetRealStartDate()).Select(e => e.Name)).ToArray();
			var eventsAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, eventNames);
			eventSelector.Adapter = eventsAdapter;

			var defaultEventId = settings.GetInt("defaultEvent", -1);
			var defaultEvent = events.FirstOrDefault(e => e.Id == defaultEventId);
			if (defaultEvent != null)
			{
				eventSelector.SetSelectedItem(defaultEvent.Name);
            } 
            else
            {
                eventSelector.SetSelectedItem(Consts.NoEvent);
            }

			eventSelector.ItemSelected += (_, __) =>
			{
				using (var edit = settings.Edit())
				{
                    var selectedEventName = eventSelector.SelectedItem.ToString();
                    var selectedEvent = events.FirstOrDefault(e => e.Name == selectedEventName);
                    var selectedEventId = selectedEvent != null ? selectedEvent.Id : -1;
                    edit.PutInt("defaultEvent", selectedEventId);
					edit.Commit();
				}
			};
		}
    }
}

