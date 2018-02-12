using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class IndexerExpression : Expression
    {
        internal IndexerExpression(Type type, Expression subject, Expression target)
        {
            Type = type;
            Subject = subject;
            Target = target as ConstantExpression;
        }

        public override ExpressionType NodeType => (ExpressionType) WiqlExpressionType.Indexer;
        public override Type Type { get; }

        internal Expression Subject { get; private set;}
        internal ConstantExpression Target { get; private set; }
    }
}