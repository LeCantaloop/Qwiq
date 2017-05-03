using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Exceptions
{
    internal class ExceptionMapper : IExceptionMapper
    {
        private readonly IEnumerable<IExceptionExploder> _exploders;
        private readonly IEnumerable<IExceptionMapper> _mappers;

        public ExceptionMapper([NotNull] IEnumerable<IExceptionExploder> exploders, [NotNull] IEnumerable<IExceptionMapper> mappers)
        {
            Contract.Requires(exploders != null);
            Contract.Requires(mappers != null);

            _exploders = exploders ?? throw new ArgumentNullException(nameof(exploders));
            _mappers = mappers ?? throw new ArgumentNullException(nameof(mappers));
        }

        public Exception Map(Exception ex)
        {
            return MapImpl(ex) ?? ex;
        }

        private Exception MapImpl(Exception ex)
        {
            var q = new Queue<Exception>();
            q.Enqueue(ex);

            while (q.Count > 0)
            {
                var item = q.Dequeue();
                var mappedException = _mappers
                                        .Select(mapper => mapper.Map(item))
                                        .FirstOrDefault(me => me != null);

                if (mappedException == null)
                {
                    foreach (var childException in _exploders
                        .Select(exceptionExploder => exceptionExploder.Explode(item))
                        .SelectMany(e => e))
                    {
                        q.Enqueue(childException);
                    }
                }
                else
                {
                    return mappedException;
                }
            }

            return null;
        }
    }
}

