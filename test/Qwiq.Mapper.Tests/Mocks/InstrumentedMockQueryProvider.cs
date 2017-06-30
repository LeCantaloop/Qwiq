using System;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Mapper.Mocks
{
    public class InstrumentedMockQueryProvider : IQueryProvider, IDisposable
    {
        private readonly IQueryProvider _innerProvider;

        public InstrumentedMockQueryProvider(IQueryProvider innerProvider)
        {
            _innerProvider = innerProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            CreateQueryCallCount += 1;
            return _innerProvider.CreateQuery(expression);
        }
        public int CreateQueryCallCount { get; private set; }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            CreateQueryTCallCount += 1;
            return _innerProvider.CreateQuery<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            ExecuteCallCount += 1;
            return _innerProvider.Execute(expression);
        }
        public int ExecuteCallCount { get; private set; }

        public TResult Execute<TResult>(Expression expression)
        {
            ExecuteTCallCount += 1;
            return _innerProvider.Execute<TResult>(expression);
        }
        public int ExecuteTCallCount { get; private set; }

        public int CreateQueryTCallCount { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_innerProvider as IDisposable)?.Dispose();
            }
        }
    }
}

