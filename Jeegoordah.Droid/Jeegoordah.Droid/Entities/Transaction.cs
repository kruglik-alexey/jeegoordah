using System;
using System.Collections.Generic;
using System.Linq;

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
		public string Comment;
    }
}

