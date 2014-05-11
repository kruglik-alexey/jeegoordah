using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class ExchangeRate : Identifiable
    {
        public virtual Currency Currency { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual decimal Rate { get; set; }
    }

    class ExchangeRateMap : ClassMap<ExchangeRate>
    {
        public ExchangeRateMap()
        {
            Id(x => x.Id);
            References(x => x.Currency).Not.Nullable();
            Map(x => x.Date).Not.Nullable();
            Map(x => x.Rate).Not.Nullable();            
        }
    }
}
