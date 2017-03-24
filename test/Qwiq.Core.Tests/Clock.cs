using System;
using System.Runtime.InteropServices;

namespace Microsoft.Qwiq.Core.Tests
{
    public static class Clock
    {
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long value);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long value);

        public static long GetTimestamp()
        {
            QueryPerformanceCounter(out long num);
            return num;
        }

        private static long Frequency

        {
            get
            {
                QueryPerformanceFrequency(out long num);
                return num;
            }
        }

        public static TimeSpan GetTimeSpan(long start, long stop)
        {
            return GetTimeSpan(start, stop, Frequency);
        }

        private static TimeSpan GetTimeSpan(long start, long stop, long frequency)
        {
            var seconds = 1.0d * (double)Math.Max(0L, stop - start) / (double)frequency;
            var ticks = (long)Math.Round(seconds * 10000000d);
            return TimeSpan.FromTicks(ticks);
        }
    }
}