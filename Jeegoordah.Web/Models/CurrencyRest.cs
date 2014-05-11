using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jeegoordah.Core.DL.Entity;

namespace Jeegoordah.Web.Models
{
    public class CurrencyRest
    {
        public CurrencyRest(Currency currency)
        {
            Id = currency.Id;
            Name = currency.Name;
            Accuracy = currency.Accuracy;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Accuracy { get; set; }

        public bool IsBase
        {
            get { return Name == "USD"; }
        }
    }
}