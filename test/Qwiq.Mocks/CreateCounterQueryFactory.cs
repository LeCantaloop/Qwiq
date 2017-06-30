using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mocks
{
    public class CreateCounterQueryFactory : IQueryFactory
    {
        [NotNull] private readonly IQueryFactory _delegate;
        [NotNull] private readonly IList<string> _queries;

        public CreateCounterQueryFactory([NotNull] IQueryFactory @delegate)
        {
            _delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
            _queries = new List<string>();
        }

        public int CreateCallCount { get; private set; }
        
        public IEnumerable<string> Queries => _queries;

        public IQuery Create(string wiql, bool dayPrecision = false)
        {
            CreateCallCount++;
            _queries.Add(wiql);
            return _delegate.Create(wiql, dayPrecision);
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            CreateCallCount++;
            _queries.Add(wiql);
            return _delegate.Create(ids, wiql);
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            CreateCallCount++;
            return _delegate.Create(ids, asOf);
        }
    }
}