using System;
using System.Collections.Generic;
using System.Diagnostics;

using Humanizer;

namespace Microsoft.Qwiq.Tests.Common
{
    public abstract class TimedContextSpecification : ContextSpecification
    {
        public IDictionary<string, TimeSpan> Durations { get; } = new Dictionary<string, TimeSpan>(StringComparer.OrdinalIgnoreCase);

        protected void TimedAction(Action action, string category, string userMessage)
        {
            var start = Clock.GetTimestamp();
            try
            {
                action();
            }
            finally
            {
                var stop = Clock.GetTimestamp();
                var duration = Clock.GetTimeSpan(start, stop);
                Debug.Print("{0}: {1} {2}", category, duration.Humanize(), userMessage);
                if (!Durations.ContainsKey(category))
                {
                    Durations[category] = TimeSpan.Zero;
                }

                Durations[category] += duration;
            }
        }

        protected T TimedAction<T>(Func<T> action, string category, string userMessage)
        {
            var start = Clock.GetTimestamp();
            try
            {
                return action();
            }
            finally
            {
                var stop = Clock.GetTimestamp();
                var duration = Clock.GetTimeSpan(start, stop);
                Debug.Print("{0}: {1} {2}", category, duration.Humanize(), userMessage);
                if (!Durations.ContainsKey(category))
                {
                    Durations[category] = TimeSpan.Zero;
                }

                Durations[category] += duration;
            }
        }

        public override void Cleanup()
        {
            foreach (var category in Durations)
            {
                Debug.Print("{0}: {1} Total", category.Key, category.Value.Humanize());
            }

            var hasSoap = Durations.TryGetValue("SOAP", out TimeSpan soapTime);
            var hasRest = Durations.TryGetValue("REST", out TimeSpan restTime);

            if (!hasSoap || !hasRest) return;

            var p50 = new TimeSpan((long)Math.Round(soapTime.Ticks * 1.5));
            var p75 = new TimeSpan((long)Math.Round(soapTime.Ticks * 1.25));
            var p90 = new TimeSpan((long)Math.Round(soapTime.Ticks * 1.10));
            var p95 = new TimeSpan((long)Math.Round(soapTime.Ticks * 1.05));

            var restBetterThanp50 = restTime <= p50;
            var restBetterThanp75 = restTime <= p75;
            var restBetterThanp90 = restTime <= p90;
            var restBetterThanp95 = restTime <= p95;

            Debug.WriteLineIf(!restBetterThanp50, $"REST is not better than 50% of SOAP time. Expected {restTime.Humanize()} < {p50.Humanize()}");
            Debug.WriteLineIf(!restBetterThanp75, $"REST is not better than 75% of SOAP time. Expected {restTime.Humanize()} < {p75.Humanize()}");
            Debug.WriteLineIf(!restBetterThanp90, $"REST is not better than 90% of SOAP time. Expected {restTime.Humanize()} < {p90.Humanize()}");
            Debug.WriteLineIf(!restBetterThanp95, $"REST is not better than 95% of SOAP time. Expected {restTime.Humanize()} < {p95.Humanize()}");
        }
    }
}