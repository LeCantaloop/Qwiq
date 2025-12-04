using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class IndexerExpression : Expression
    {
        internal IndexerExpression(Type type, Expression? subject, Expression target)
        {
            Type = type;
            Subject = subject;
            Target = target as ConstantExpression ?? throw new ArgumentException("Target must be a ConstantExpression", nameof(target));
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.Indexer;
        public override Type Type { get; }

        internal Expression? Subject { get; }
        internal ConstantExpression Target { get; }
    }
}