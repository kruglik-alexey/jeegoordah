using Android.Content;

namespace Jeegoordah.Droid
{
	public interface ISharedPreferencesProvider
	{
		ISharedPreferences Get(string name);
	}		
}
