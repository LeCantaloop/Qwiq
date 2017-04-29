using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Qwiq.Linq.Fragments;

namespace Microsoft.Qwiq.Linq
{
    /// <summary>
    /// This guy holds the results of expression visiting. He allows us to keep lists of sub-expressions
    /// so we can join them. The idea is that rather than trying to deal with things like chained .Where() clauses
    /// while visiting the expression, we can just gather the expressions as we go then arrange things accordingly later.
    /// </summary>
    public class TranslatedQuery
    {
        internal IFragment Select { get; set; }
        public Type UnderlyingQueryType { get; set; }
        internal Queue<IFragment> WhereClauses { get; private set; }
        internal Queue<IFragment> ThenOrderClauses { get; private set; }
        public DateTime? AsOfDateTime { get; set; }
        public IList<LambdaExpression> Projections { get; private set; }

        public bool WillEverHaveResults()
        {
            return Select.IsValid() && WhereClauses.All(fragment => fragment.IsValid()) &&
                   ThenOrderClauses.All(fragment => fragment.IsValid());
        }

        public TranslatedQuery()
        {
            Select = null;
            UnderlyingQueryType = null;
            AsOfDateTime = null;
            WhereClauses = new Queue<IFragment>();
            ThenOrderClauses = new Queue<IFragment>();
            Projections = new List<LambdaExpression>();
        }

        public string ToQueryString()
        {
            var result = Select.Get(UnderlyingQueryType);
            result += BuildWhereString();
            result += BuildOrderString();
            result += BuildAsOfString();
            return result;
        }

        private string BuildWhereString()
        {
            var result = "";

            if (WhereClauses.Count > 0)
            {
                result += " WHERE (";
                result += string.Join(" AND ", WhereClauses.Select(wc => wc.Get(UnderlyingQueryType)));
                result += ")";
            }

            return result;
        }

        private string BuildOrderString()
        {
            var result = "";

            if (ThenOrderClauses.Count > 0)
            {
                result += " ORDER BY ";
                result += string.Join(", ", ThenOrderClauses.Select(toc => toc.Get(UnderlyingQueryType)));
            }

            return result;
        }

        private string BuildAsOfString()
        {
            var result = "";

            if (AsOfDateTime.HasValue)
            {
                AsOfDateTime = AsOfDateTime.Value.ToUniversalTime();

                result += " ASOF ";
                result += "'" + AsOfDateTime.Value.ToString("u") + "'";
            }

            return result;
        }
    }
}

