using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Jeegoordah.Droid.Entities;

namespace Jeegoordah.Droid.UI
{
	public class BroMultiSelector : DialogFragment
    {
		private readonly IList<Tuple<Bro, bool>> bros;
		private readonly Action<IList<Tuple<Bro, bool>>> ok;

		public BroMultiSelector(IList<Tuple<Bro, bool>> bros, Action<IList<Tuple<Bro, bool>>> ok)
		{
			this.ok = ok;
			this.bros = new List<Tuple<Bro, bool>>(bros);        
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var builder = new AlertDialog.Builder(Activity)
				.SetMultiChoiceItems(bros.Select(b => b.Item1.Name).ToArray(), bros.Select(b => b.Item2).ToArray(), 
					(_, a) => bros[a.Which] = Tuple.Create(bros[a.Which].Item1, a.IsChecked))
				.SetPositiveButton("OK", (_, __) => ok(bros))
				.SetNegativeButton("Cancel", (_, __) => {});
			return builder.Create();
		}
    }
}

