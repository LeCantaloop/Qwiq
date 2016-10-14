using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Exceptions
{
    internal class ExceptionMapper : IExceptionMapper
    {
        private readonly IEnumerable<IExceptionExploder> _exploders;
        private readonly IEnumerable<IExceptionMapper> _mappers;

        public ExceptionMapper(IEnumerable<IExceptionExploder> exploders, IEnumerable<IExceptionMapper> mappers)
        {
            _exploders = exploders;
            _mappers = mappers;
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

