using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Jeegoordah.Web
{
    public static class JsonDate
    {
        public static DateTime Parse(string jsonDate)
        {
            return DateTime.ParseExact(jsonDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        public static string ToString(DateTime date)
        {
            return date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
    }
}