using System.Diagnostics;
using NHibernate;
using NHibernate.SqlCommand;

namespace Jeegoordah.Core.DL
{
	class LoggingInterceptor : EmptyInterceptor
	{
		public override SqlString OnPrepareStatement(SqlString sql)
		{
			Trace.WriteLine(sql.ToString());
			return base.OnPrepareStatement(sql);
		}
	}
}