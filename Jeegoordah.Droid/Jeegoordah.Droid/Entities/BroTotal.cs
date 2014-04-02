using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeegoordah.Droid.Entities
{
    public class BroTotal
    {
		public int Bro;
		public IList<CurrencyAmount> Amounts;

		public BroTotal Clone()
		{
			return new BroTotal
			{
				Bro = Bro,
				Amounts = Amounts.Select(a => a.Clone()).ToList()
			};
		}
    }
}

