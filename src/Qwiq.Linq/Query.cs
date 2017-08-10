using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Linq
{
    // Boiler plate
    // Developed from http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx
    [DebuggerStepThrough]
    public sealed class Query<T> : IOrderedQueryable<T>
    {
        private readonly IWiqlQueryBuilder _builder;

        private readonly Expression _expression;

        private readonly IQueryProvider _provider;

        public Query([NotNull] IQueryProvider provider, [NotNull] IWiqlQueryBuilder builder)
        {
            Contract.Requires(provider != null);
            Contract.Requires(builder != null);

            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _expression = Expression.Constant(this);
        }

        public Query([NotNull] IQueryProvider provider, [NotNull] IWiqlQueryBuilder builder, [NotNull] Expression expression)
        {
            Contract.Requires(provider != null);
            Contract.Requires(builder != null);
            Contract.Requires(expression != null);

            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type)) throw new ArgumentOutOfRangeException(nameof(expression));

            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        Type IQueryable.ElementType => typeof(T);

        Expression IQueryable.Expression => _expression;

        IQueryProvider IQueryable.Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            var enumerable = _provider.Execute<IEnumerable<T>>(_expression);
            return enumerable.GetEnumerator();
        }

        public override string ToString()
        {
            return _builder.BuildQuery(_expression).ToQueryString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerable = (IEnumerable)_provider.Execute(_expression);
            return enumerable.GetEnumerator();
        }
    }
}