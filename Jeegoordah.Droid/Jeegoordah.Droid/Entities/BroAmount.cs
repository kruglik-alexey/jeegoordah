using System;

namespace Jeegoordah.Droid.Entities
{
    public class BroAmount
    {
        public int Bro;
        public decimal Amount;

        public BroAmount Clone() 
        {
            return new BroAmount { Bro = Bro, Amount = Amount };
        }
    }
}

