using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Event
    {
        public Event()
        {
            Bros = new HashSet<Bro>();
            Transactions = new List<Transaction>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime StartDate { get; set; }        
        public virtual string Description { get; set; }
        public virtual ICollection<Bro> Bros { get; set; }
        public virtual IList<Transaction> Transactions { get; set; }
    }

    class EventMap : ClassMap<Event>
    {
        public EventMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Unique();
            Map(x => x.StartDate).Not.Nullable();            
            Map(x => x.Description).Not.Nullable();
            HasManyToMany(x => x.Bros).AsSet().Table("BroEvents");
            HasMany(x => x.Transactions).Inverse();
        }
    }
}
