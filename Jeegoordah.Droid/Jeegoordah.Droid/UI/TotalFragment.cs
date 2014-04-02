using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Jeegoordah.Droid.Entities;
using Jeegoordah.Droid.Repositories;

namespace Jeegoordah.Droid.UI
{
    public class TotalFragment : Fragment
    {
		private readonly LocalRepository repository;

		public TotalFragment(LocalRepository repository)
		{
			this.repository = repository;			
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var list = (ListView)inflater.Inflate(Resource.Layout.Total, container, false);
			list.Adapter = new TotalAdapter(Activity, Resource.Layout.TotalRow, GetTotal().Result, repository.GetBros().Result, repository.GetCurencies().Result);
			return list;
		}

		async private Task<IList<BroTotal>> GetTotal()
		{
			return TotalCalculator.Calculate(await repository.GetTotal(), repository.GetPendingTransactions());
		}
    }
}

