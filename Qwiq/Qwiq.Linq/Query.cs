using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq
{
    // Boiler plate
    // Developed from http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx
    public sealed class Query<T> : IOrderedQueryable<T>
    {
        private readonly IQueryProvider _provider;
        private readonly IWiqlQueryBuilder _builder;
        private readonly Expression _expression;

        public Query(IQueryProvider provider, IWiqlQueryBuilder builder)
        {
            if (provider == null) { throw new ArgumentNullException("provider"); }

            _provider = provider;
            _builder = builder;
            _expression = Expression.Constant(this);
        }

        public Query(IQueryProvider provider, IWiqlQueryBuilder builder, Expression expression)
        {
            if (provider == null) { throw new ArgumentNullException("provider"); }
            if (expression == null) { throw new ArgumentNullException("expression"); }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type)) { throw new ArgumentOutOfRangeException("expression"); }

            _provider = provider;
            _builder = builder;
            _expression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return _expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerable = _provider.Execute<IEnumerable<T>>(_expression);
            var enumerator = enumerable.GetEnumerator();
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var enumerable = (IEnumerable)_provider.Execute(_expression);
            var enumerator = enumerable.GetEnumerator();
            return enumerator;
        }

        public override string ToString()
        {
            return _builder.BuildQuery(_expression).ToQueryString();
        }
    }
}
