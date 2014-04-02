using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.UI
{
	class TotalAdapter : ArrayAdapter<BroTotal>
	{
		private readonly int viewResourceId;
		private readonly IList<BroTotal> items;
		private readonly IList<Bro> bros;
		private readonly IList<Currency> currencies;
		private readonly CultureInfo culture;

		public TotalAdapter(Context context, int viewResourceId, IList<BroTotal> items, IList<Bro> bros, IList<Currency> currencies) : base(context, viewResourceId, items)
		{
			this.currencies = currencies;
			this.bros = bros;
			this.items = items;
			this.viewResourceId = viewResourceId;

			culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			culture.NumberFormat.NumberGroupSeparator = " ";			
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			if(row == null)
			{
				LayoutInflater inflater = ((Activity)Context).LayoutInflater;
				row = inflater.Inflate(viewResourceId, parent, false);
			}

			row.FindViewById<TextView>(Resource.Id.BroName).Text = bros.First(b => b.Id == items[position].Bro).Name;

			var amountsBuilder = new SpannableStringBuilder();
			foreach (CurrencyAmount currencyAmount in items[position].Amounts)
			{
				var currency = currencies.First(c => c.Id == currencyAmount.Currency);
				var amount = currencyAmount.Amount.WithAccuracy(currency.Accuracy);
				if (amount == 0)
					continue;

				var amountString = new SpannableString(String.Format(culture, "{0:#,#} {1}", amount, currency.Name));

				ForegroundColorSpan foregroundColor;
				foregroundColor = amount > 0 ? new ForegroundColorSpan(Color.Green) : new ForegroundColorSpan(Color.Red);
				amountString.SetSpan(foregroundColor, 0, amountString.Length(), SpanTypes.Composing);
				amountsBuilder.Append(amountString);
				amountsBuilder.Append("\n");
			}

			if (amountsBuilder.Length() > 0)
			{
				amountsBuilder.Delete(amountsBuilder.Length() - 1 , amountsBuilder.Length());					
			}
			else
			{
				amountsBuilder.Append("0");
				amountsBuilder.SetSpan(new ForegroundColorSpan(Color.Gray), 0, 1, SpanTypes.Composing);
			}

			row.FindViewById<TextView>(Resource.Id.BroAmounts).SetText(amountsBuilder, TextView.BufferType.Spannable);

			return row;
		}
	}
}
