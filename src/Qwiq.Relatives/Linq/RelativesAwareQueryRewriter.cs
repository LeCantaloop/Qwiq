using System.Linq.Expressions;
using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Relatives.WiqlExpressions;

namespace Microsoft.Qwiq.Relatives.Linq
{
    public class RelativesAwareQueryRewriter : QueryRewriter
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(QueryExtensions) && node.Method.Name == "Parents")
            {
                Visit(node.Arguments[0]);
                var methodGenericTypes = node.Method.GetGenericArguments();
                return new ParentsExpression(node.Type, methodGenericTypes[0], methodGenericTypes[1]);
            }

            if (node.Method.DeclaringType == typeof(QueryExtensions) && node.Method.Name == "Children")
            {
                Visit(node.Arguments[0]);
                var methodGenericTypes = node.Method.GetGenericArguments();
                return new ChildrenExpression(node.Type, methodGenericTypes[0], methodGenericTypes[1]);
            }

            return base.VisitMethodCall(node);
        }
    }
}
