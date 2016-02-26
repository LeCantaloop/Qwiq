using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.IE.Qwiq.Linq.WiqlExpressions;

namespace Microsoft.IE.Qwiq.Linq.Visitors
{
    // Developed from http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx
    /// <summary>
    /// This class is going to visit each node in the query's expression
    /// tree and for expressions that don't map well into WIQL, replace those nodes with
    /// new expressions that map more closely to WIQL. Other nodes are reduced if possible
    /// and the rest are left alone.
    /// </summary>
    public class QueryRewriter : ExpressionVisitor
    {
        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Select")
            {
                var select = Visit(node.Arguments[0]);
                var projection = Visit(StripQuotes(node.Arguments[1])) as LambdaExpression;

                if (projection == null)
                {
                    throw new NotSupportedException(String.Format("Performing a select without a lambda is not supported"));
                }

                return new SelectExpression(node.Type, select, projection);
            }

            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                var select = Visit(node.Arguments[0]);
                var filter = Visit(StripQuotes(node.Arguments[1]));

                return new WhereExpression(node.Type, select, filter);
            }

            if (node.Method.DeclaringType == typeof(Queryable) && (node.Method.Name == "OrderBy" || node.Method.Name == "ThenBy"))
            {
                var source = Visit(node.Arguments[0]);
                var orderSelector = Visit(StripQuotes(node.Arguments[1]));

                return new OrderExpression(node.Type, source, orderSelector, OrderOptions.Ascending);
            }

            if (node.Method.DeclaringType == typeof(Queryable) && (node.Method.Name == "OrderByDescending" || node.Method.Name == "ThenByDescending"))
            {
                var source = Visit(node.Arguments[0]);
                var orderSelector = Visit(StripQuotes(node.Arguments[1]));

                return new OrderExpression(node.Type, source, orderSelector, OrderOptions.Descending);
            }

            if (node.Method.DeclaringType == typeof(String) && node.Method.Name == "StartsWith")
            {
                var subject = Visit(node.Object);
                var target = Visit(node.Arguments[0]);

                return new UnderExpression(node.Type, subject, target);
            }

            // This is a contains used to see if a value is in a list, such as: bug => aliases.Contains(bug.AssignedTo)
            if (node.Method.DeclaringType == typeof(Enumerable) && node.Method.Name == "Contains")
            {
                var subject = Visit(node.Arguments[1]);
                var target = Visit(node.Arguments[0]);

                return new InExpression(node.Type, subject, target);
            }

            // This is a contains used to do substring matching on a value, such as: bug => bug.Status.Contains("Approved")
            if (node.Method.DeclaringType == typeof(String) && node.Method.Name == "Contains")
            {
                var subject = Visit(node.Object);
                var target = Visit(node.Arguments[0]);

                return new ContainsExpression(node.Type, subject, target);
            }

            if (node.Method.DeclaringType == typeof(QueryExtensions) && node.Method.Name == "AsOf")
            {
                Visit(node.Arguments[0]);
                var time = (ConstantExpression)Visit(node.Arguments[1]);

                return new AsOfExpression(node.Type, (DateTime)time.Value);
            }

            if (node.Method.Name == "ToUpper" || node.Method.Name == "ToUpperInvariant" || node.Method.Name == "ToLower" || node.Method.Name == "ToLowerInvariant")
            {
                throw new NotSupportedException(String.Format("The method {0} is not supported. Queries are case insensitive, so string comparisons should use the regular operators ( ==, > <=, etc.)", node.Method.Name));
            }

            // Unknown method call
            throw new NotSupportedException(String.Format("The method '{0}' is not supported", node.Method.Name));
        }
    }
}
