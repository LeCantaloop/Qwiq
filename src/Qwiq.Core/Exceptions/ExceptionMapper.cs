using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Qwiq.Exceptions
{
    internal class ExceptionMapper : IExceptionMapper
    {
        private readonly IExceptionExploder[] _exploders;
        private readonly IExceptionMapper[] _mappers;

        public ExceptionMapper(
            [NotNull] IExceptionExploder[] exploders,
            [NotNull] IExceptionMapper[] mappers)
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

        [CanBeNull]
        private Exception MapImpl(Exception ex)
        {
            var q = new Queue<Exception>();
            q.Enqueue(ex);

            while (q.Count > 0)
            {
                var item = q.Dequeue();
                Exception mappedException = null;
                for (var i = 0; i < _mappers.Length; i++)
                {
                    var mapper = _mappers[i];
                    var me = mapper.Map(item);
                    if (me == null) continue;
                    mappedException = me;
                    break;
                }

                if (mappedException == null)
                {
                    for (var i = 0; i < _exploders.Length; i++)
                    {
                        var exceptionExploder = _exploders[i];
                        var exceptions = exceptionExploder.Explode(item);

                        if (exceptions == null) continue;

                        for (var j = 0; j < exceptions.Count; j++)
                        {
                            var childException = exceptions[j];
                            q.Enqueue(childException);
                        }
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