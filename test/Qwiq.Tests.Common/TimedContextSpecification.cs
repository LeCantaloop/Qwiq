using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class TimedContextSpecification : ContextSpecification
    {
        private readonly IDictionary<string, TimeSpan> _durations = new Dictionary<string, TimeSpan>(StringComparer.OrdinalIgnoreCase);

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
                Debug.Print("{0}: {1} {2}", category, duration, userMessage);
                if (!_durations.ContainsKey(category))
                {
                    _durations[category] = TimeSpan.Zero;
                }

                _durations[category] += duration;
            }
        }

        public override void Cleanup()
        {
            foreach (var category in _durations)
            {
                Debug.Print("{0}: {1}", category.Key, category.Value);
            }
        }
    }
}