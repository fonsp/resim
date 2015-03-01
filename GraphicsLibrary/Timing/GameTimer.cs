//using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GraphicsLibrary.Timing
{
	public class GameTimer
	{

		/*[DllImport("kernel32.dll")]
		extern static int QueryPerformanceCounter(ref long l);
		[DllImport("kernel32.dll")]
		extern static int QueryPerformanceFrequency(ref long l);

		private long startTime;
		private long stopTime;
		private readonly long clockFrequency;*/

		Stopwatch stopwatch = new Stopwatch();

		public GameTimer()
		{
			//QueryPerformanceFrequency(ref clockFrequency);
			stopwatch.Start();
		}

		public double GetElapsedTimeInSeconds()
		{
			/*QueryPerformanceCounter(ref stopTime);
			double value = ((stopTime - startTime) * 1000000.0 / clockFrequency) / 1000000;
			QueryPerformanceCounter(ref startTime);*/
			stopwatch.Reset();
			stopwatch.Start();
			double value = stopwatch.ElapsedMilliseconds / 1000.0;
			return value;
		}
	}
}