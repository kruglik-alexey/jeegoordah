using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jeegoordah.Core
{
    public static class Helper
    {        
        public static string F(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T element in collection)
            {
                action(element);
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            range.ForEach(collection.Add);
        }

		public static string ToJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
    }
}
