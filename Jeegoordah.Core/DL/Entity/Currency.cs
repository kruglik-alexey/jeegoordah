using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Currency
    {        
        public virtual int Id { get; set; }        
        public virtual string Name { get; set; }        
    }

    class CurrencyMap : ClassMap<Currency>
    {
        public CurrencyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Unique();
        }
    }
}
