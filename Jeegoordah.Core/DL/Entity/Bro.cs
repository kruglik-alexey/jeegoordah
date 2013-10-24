using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Bro
    {
        public Bro()
        {
            Events = new List<Event>();
            Transactions = new List<Transaction>();
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual List<Event> Events { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
    }

    class BroMap : ClassMap<Bro>
    {
        public BroMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Unique();
            HasManyToMany(x => x.Events);
            HasManyToMany(x => x.Transactions);
        }
    }
}
