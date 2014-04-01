using System;
using Android.Content;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using Jeegoordah.Droid.Entities;
using System.Globalization;

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

		public static void SetSelectedItem(this Spinner spinner, Java.Lang.Object item)
		{
			var adapter = spinner.Adapter as ArrayAdapter;
			if (adapter == null)
				throw new InvalidOperationException();
			spinner.SetSelection(adapter.GetPosition(item));
		}

		public static T GetEntityFromSettings<T>(IList<T> entities, ISharedPreferences settings, string key) where T : Identifiable
		{
			var id = settings.GetInt(key, -1);
			return entities.FirstOrDefault(e => e.Id == id);		
		}

		public static decimal WithAccuracy(this decimal number, int accuracy) 
		{
			if (accuracy == 0)
				return (int)number;
			if (accuracy < 0)
				throw new ArgumentOutOfRangeException();
			var factor = Math.Pow(10, accuracy);
			return (decimal)(Math.Round((double)number / factor) * factor);
		}
    }
}

