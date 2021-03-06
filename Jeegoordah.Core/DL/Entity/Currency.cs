﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace Jeegoordah.Core.DL.Entity
{
    public class Currency : Identifiable
    {        
        public virtual string Name { get; set; }        
        public virtual int Accuracy { get; set; }
        public virtual bool IsHidden { get; set; }

        public virtual bool IsBase()
        {
            return Name == "USD";
        }
    }

    class CurrencyMap : ClassMap<Currency>
    {
        public CurrencyMap()
        {
            Id(x => x.Id);
            Map(x => x.Name).Not.Nullable().Unique();
            Map(x => x.Accuracy).Not.Nullable();
            Map(x => x.IsHidden).Not.Nullable();
        }
    }
}
