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
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual IList<Event> Events { get; set; }
    }

    class BroMap : ClassMap<Bro>
    {
        public BroMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Unique();
            HasManyToMany(x => x.Events).Table("BroEvents");
        }
    }
}
