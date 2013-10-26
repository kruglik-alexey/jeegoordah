using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class EventRest
    {
        public EventRest()
        {
            Bros = new List<int>();
        }

        public EventRest(Event source)
        {
            Id = source.Id;
            Name = source.Name;
            Description = source.Description;
            StartDate = JsonDate.ToString(source.StartDate);
            Bros = source.Bros.Select(b => b.Id).ToList();
            Transactions = new List<Transaction>(source.Transactions);
            Transactions.ForEach(t => t.Event = null);
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public List<int> Bros { get; set; }
        public List<Transaction> Transactions { get; set; }

        public void ToDataObject(Event target)
        {
            if (Id.HasValue)
            {
                target.Id = Id.Value;
            }
            target.Name = Name;
            target.StartDate = JsonDate.Parse(StartDate);
            target.Description = Description;
            var oldBros = target.Bros.Select(b => b.Id).ToList();
            var addedBros = Bros.Except(oldBros).ToList();
            var removedBros = oldBros.Except(Bros).ToList();
//            target.Bros.AddRange(addedBros.Select(b => new Bro { Id = b }));
//            target.Bros.RemoveAll(b => removedBros.Contains(b.Id));
        }
    }
}