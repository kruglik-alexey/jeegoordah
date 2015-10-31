using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
	public class Notification : Identifiable
	{
		public virtual IList<Bro> Bros { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual string Content { get; set; }
	}

	public class NotificationMap : ClassMap<Notification>
	{
		public NotificationMap()
		{
			Id(x => x.Id);
			Map(x => x.Date).Not.Nullable();
			Map(x => x.Content).Not.Nullable();
			HasManyToMany(x => x.Bros).Table("BroNotifications");
		}
	}
}