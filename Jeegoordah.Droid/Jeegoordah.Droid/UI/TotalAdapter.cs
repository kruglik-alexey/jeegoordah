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
    class TotalAdapter : ArrayAdapter<BroAmount>
	{
		private readonly int viewResourceId;
        private readonly IList<BroAmount> items;
		private readonly IList<Bro> bros;
        private readonly Currency baseCurrency;
		private readonly CultureInfo culture;

        public TotalAdapter(Context context, int viewResourceId, IList<BroAmount> items, IList<Bro> bros, Currency baseCurrency) : base(context, viewResourceId, items)
		{
            this.baseCurrency = baseCurrency;
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
            var amount = items[position].Amount.WithAccuracy(baseCurrency.Accuracy);
            var amountString = new SpannableString(String.Format(culture, "{0:0.#} {1}", amount, baseCurrency.Name));

            ForegroundColorSpan foregroundColor;
            foregroundColor = amount > 0 ? new ForegroundColorSpan(Color.Green) : (amount < 0 ? new ForegroundColorSpan(Color.Red) : new ForegroundColorSpan(Color.Gray));
            amountString.SetSpan(foregroundColor, 0, amountString.Length(), SpanTypes.Composing);
            amountsBuilder.Append(amountString);

			row.FindViewById<TextView>(Resource.Id.BroAmounts).SetText(amountsBuilder, TextView.BufferType.Spannable);
            return row;
		}
	}
}
