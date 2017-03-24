using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Microsoft.Qwiq.Rest
{
    internal class QueryFactory : IQueryFactory
    {
        private readonly WorkItemTrackingHttpClient _store;

        private QueryFactory(WorkItemTrackingHttpClient store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public static IQueryFactory GetInstance(WorkItemTrackingHttpClient store)
        {
            return new QueryFactory(store);
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            try
            {
                var p = Parser.ParseSyntax(wiql);
                return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(new QueryProxy(p, _store));
            }
            catch (SyntaxException e)
            {
                throw new ValidationException(e);
            }
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));

            try
            {
                var p = Parser.ParseSyntax(wiql);
                if (p.Where != null || p.OrderBy != null)
                {
                    throw new ValidationException("The WHERE and ORDER BY clauses of a query string are not supported on a parameterized query.");
                }

                // TODO: Check that the WIQL is not a LINK or TREE query

                // We will use the SELECT information to determine the fields to return

                return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(new QueryProxy(ids, p, _store));
            }
            catch (SyntaxException e)
            {
                throw new ValidationException(e);
            }
        }
    }
}