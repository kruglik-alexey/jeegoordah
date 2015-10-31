using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Bro : Identifiable
    {
        public Bro()
        {
            Events = new List<Event>();
        }

        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual IList<Event> Events { get; set; }        
        public virtual IList<Notification> Notifications { get; set; }        
    }

    class BroMap : ClassMap<Bro>
    {
        public BroMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Unique();
	        Map(x => x.Email).Nullable();
            HasManyToMany(x => x.Events).Table("BroEvents").Inverse();
			HasManyToMany(x => x.Notifications).Table("BroNotifications").Inverse();
		}
    }
}
