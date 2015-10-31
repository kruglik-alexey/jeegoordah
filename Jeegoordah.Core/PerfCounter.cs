using System;
using System.Diagnostics;
using Jeegoordah.Core.Logging;

namespace Jeegoordah.Core
{
	public class PerfCounter : IDisposable
	{
		private readonly string _category;
		private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

		public PerfCounter(string category)
		{
			_category = category;
		}

		public void Dispose()
		{
			_stopwatch.Stop();
			new Logger($"perf-{_category}").I(_stopwatch.ElapsedMilliseconds.ToString());
		}
	}
}