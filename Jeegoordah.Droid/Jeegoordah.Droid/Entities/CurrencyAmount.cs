using System;

namespace Jeegoordah.Droid.Entities
{
    public class CurrencyAmount
    {      
		public int Currency;
		public decimal Amount;

		public CurrencyAmount Clone() 
		{
			return new CurrencyAmount
			{
				Currency = Currency,
				Amount = Amount
			};
		}
    }
}

