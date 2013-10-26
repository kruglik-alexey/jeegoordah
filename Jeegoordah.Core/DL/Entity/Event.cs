using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Event
    {
        public Event()
        {
            Bros = new List<Bro>();
            Transactions = new List<Transaction>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<Bro> Bros { get; set; }
        public virtual IList<Transaction> Transactions { get; set; }
    }

    class EventMap : ClassMap<Event>
    {
        public EventMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Unique();
            Map(x => x.StartDate);
            Map(x => x.CreatedAt);
            Map(x => x.Description);
            HasManyToMany(x => x.Bros).Table("BroEvents");
            HasMany(x => x.Transactions);
        }
    }
}
