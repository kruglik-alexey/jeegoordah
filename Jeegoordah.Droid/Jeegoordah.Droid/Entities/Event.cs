using System;
using System.Collections.Generic;
using System.Globalization;

namespace Jeegoordah.Droid.Entities
{
    public class Event
    {      
		public int Id;
		public string Name;
		public string StartDate;
		public IList<int> Bros;

		public DateTime GetRealStartDate()
		{
			return DateTime.ParseExact(StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
		}
    }
}

