using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class ContainsExpression : Expression
    {
        internal ContainsExpression(Type type, Expression subject, Expression target)
        {
            Type = type;
            Subject = subject;
            Target = target;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.Contains;

        public override Type Type { get; }

        internal Expression Subject { get; private set; }
        internal Expression Target { get; private set; }
    }
}

