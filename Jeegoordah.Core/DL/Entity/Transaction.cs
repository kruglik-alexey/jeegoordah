using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Transaction
    {
        public Transaction()
        {
            Targets = new List<Bro>();
        }

        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual DateTime CreatedAt { get; set; }        
        public virtual Currency Currency { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual Bro Source { get; set; }
        public virtual IList<Bro> Targets { get; set; }
        public virtual Event Event { get; set; }
        public virtual string Comment { get; set; }
    }

    class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Id(x => x.Id);
            Map(x => x.Date);
            Map(x => x.CreatedAt);
            References(x => x.Currency);
            Map(x => x.Amount);
            References(x => x.Source);
            HasManyToMany(x => x.Targets).Table("TransactionTargets");
            References(x => x.Event).Nullable();
            Map(x => x.Comment);
        }
    }
}
