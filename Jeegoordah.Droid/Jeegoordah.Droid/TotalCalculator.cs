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
		public static IList<BroTotal> Calculate(IList<BroTotal> total, IList<Transaction> pendingTransactions)
		{
			var result = total.Select(t => t.Clone()).ToList();
			Func<int, int, CurrencyAmount> currencyAmount = (bro, currency) => result.First(t => t.Bro == bro).Amounts.First(c => c.Currency == currency);

			foreach (var transaction in pendingTransactions)
			{
				decimal share = transaction.Amount / transaction.Targets.Count;
				foreach (int target in transaction.Targets)
				{
					currencyAmount(target, transaction.Currency).Amount -= share;
				}

				currencyAmount(transaction.Source, transaction.Currency).Amount += transaction.Amount;
			}

			return result;
		}
    }
}

