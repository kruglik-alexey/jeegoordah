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
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid
{
	static class TotalCalculator
    {
        public static IList<BroAmount> Calculate(CurrencyTotal total, IList<Transaction> pendingTransactions)
		{
            var result = total.Totals.Select(t => t.Clone()).ToList();
            Func<int, BroAmount> broAmount = bro => result.Single(t => t.Bro == bro);

			foreach (var transaction in pendingTransactions)
			{
                decimal share = transaction.AmountInBaseCurrency / transaction.Targets.Count;
				foreach (int target in transaction.Targets)
				{
                    broAmount(target).Amount -= share;
				}

                broAmount(transaction.Source).Amount += transaction.AmountInBaseCurrency;
			}

			return result;
		}
    }
}

