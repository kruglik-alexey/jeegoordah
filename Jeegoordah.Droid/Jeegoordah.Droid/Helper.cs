using System;
using Android.Content;

namespace Jeegoordah.Droid
{
	public static class Helper
    {
		public static string F(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		public static ISharedPreferences GetSharedPreferences(this Context context, string name)
		{
			return context.GetSharedPreferences(name, FileCreationMode.Private);
		}
    }
}

