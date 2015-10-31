using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Core.BL
{
	public static class NotificationCreator
	{
		public static Notification ForCreatedTransaction(Transaction transaction)
		{
			var text = Text(transaction);
			var content = $"New transaction was created for you on {DateTime.UtcNow}\n{text}";
			return CreateNotification(transaction.Yield(), content);
		}

		public static Notification ForUpdatedTransaction(Transaction oldTransaction, Transaction newTransaction)
		{
			var oldText = Text(oldTransaction);
			var newText = Text(newTransaction);

			var content = 
$@"Your transaction was changed on {DateTime.UtcNow}
Before:
{oldText}

After:
{newText}";

			return CreateNotification(new[] {oldTransaction, newTransaction}, content);
		}

		public static Notification ForDeletedTransaction(Transaction transaction)
		{
			var text = Text(transaction);
			var content = $"Your transaction was deleted on {DateTime.UtcNow}\n{text}";
			return CreateNotification(transaction.Yield(), content);
		}

		private static Notification CreateNotification(IEnumerable<Transaction> transactions, string content)
		{
			var bros = transactions.SelectMany(TransactionBros).Distinct().ToArray();
            return new Notification {Bros = bros, Content = content, Date = DateTime.UtcNow};
		}

		private static IEnumerable<Bro> TransactionBros(Transaction transaction)
		{
			return transaction.Targets.Concat(transaction.Source.Yield()).Where(b => b.Email != null).Distinct();
		}

		private static string Text(Transaction transaction)
		{
			var r = 
$@"{transaction.Date.ToShortDateString()} - {transaction.Event?.Name ?? "[no event]"}
{transaction.Amount.ToString("#,#")} {transaction.Currency.Name}
Source	{transaction.Source.Name}
Targets	{transaction.Targets.Aggregate("", (acc, bro) => acc + bro.Name + " ")}";

			if (!string.IsNullOrWhiteSpace(transaction.Comment))
			{
				r = 
$@"{r}
Comment
{transaction.Comment}";
			}
			return r;
		}
	}
}
