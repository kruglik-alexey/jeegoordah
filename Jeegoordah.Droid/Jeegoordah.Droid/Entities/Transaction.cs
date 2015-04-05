using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Jeegoordah.Droid.Entities
{
	public class Transaction : Identifiable
    {
		public string Date;
		public int? Event;
		public int Source;
		public IList<int> Targets;
		public int Currency;
		public decimal Amount;
        public decimal Rate;
		public string Comment;

        [JsonIgnore]
        public decimal AmountInBaseCurrency
        {
            get { return Amount / Rate; }
        }
    }
}

