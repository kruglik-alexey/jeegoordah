using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class TransactionRest
    {
        public TransactionRest()
        {
            Targets = new List<int>();
        }

        public TransactionRest(Transaction transaction)
        {
            Id = transaction.Id;
            Date = JsonDate.ToString(transaction.Date);
            Amount = transaction.Amount;
            Currency = transaction.Currency.Id;
            Source = transaction.Source.Id;
            Targets = transaction.Targets.Select(t => t.Id).ToList();
            Event = transaction.Event != null ? transaction.Event.Id : (int?)null; 
            Comment = transaction.Comment;
        }

        public int? Id { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public int Source { get; set; }
        public List<int> Targets { get; set; }
        public int? Event { get; set; }
        public string Comment { get; set; }

        public void ToDataObject(Transaction target)
        {
            if (Id.HasValue)
            {
                target.Id = Id.Value;
            }
            target.Date = JsonDate.Parse(Date);
            target.Amount = Amount;
            target.Comment = Comment;
        }
    }
}