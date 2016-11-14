using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Qwiq.Linq
{
    public class TeamFoundationServerWorkItemQueryProvider : IQueryProvider
    {
        public TeamFoundationServerWorkItemQueryProvider(IWorkItemStore workItemStore,
            IWiqlQueryBuilder queryBuilder)
        {
            WorkItemStore = workItemStore;
            WiqlQueryBuilder = queryBuilder;
        }

        protected IWorkItemStore WorkItemStore { get; set; }


        protected IWiqlQueryBuilder WiqlQueryBuilder { get; set; }

        public IQueryable CreateQuery(Expression expression)
        {
            var type = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(type), new object[] { this, expression });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        public object Execute(Expression expression)
        {
            return ExecuteImpl(expression, TypeSystem.GetElementType(expression.Type));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, WiqlQueryBuilder, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isCollection = typeof(TResult).IsGenericType &&
                                typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>);

            if (isCollection)
            {
                var itemType = isCollection
                    // TResult is an IEnumerable`1 collection.
                               ? typeof(TResult).GetGenericArguments().Single()
                    // TResult is not an IEnumerable`1 collection, but a single item.
                               : typeof(TResult);
                var result = ExecuteImpl(expression, itemType);
                var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType)) as IList;

                var enumerableResult = (IEnumerable)result;
                var f = enumerableResult.GetEnumerator();
                while (f.MoveNext())
                {
                    list.Add(f.Current);
                }

                return (TResult)list;
            }

            return (TResult)ExecuteImpl(expression, typeof(TResult));
        }

        protected virtual object ExecuteImpl(Expression expression, Type itemType)
        {
            var query = WiqlQueryBuilder.BuildQuery(expression);

            var results = query.WillEverHaveResults()
                ? ExecuteRawQuery(query.UnderlyingQueryType, query.ToQueryString())
                : Activator.CreateInstance(typeof(List<>).MakeGenericType(query.UnderlyingQueryType)) as IEnumerable;

            if (query.Projections.Count > 0)
            {
                return Projector.Project(query.Projections, results.Cast<object>());
            }
            return results;
        }

        protected virtual IEnumerable ExecuteRawQuery(Type workItemType, string queryString)
        {
            var workItems = WorkItemStore.Query(queryString);
            return workItems;
        }
    }
}

