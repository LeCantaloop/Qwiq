using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class InGroupExpression : Expression
    {
        internal InGroupExpression(Type type, Expression subject, Expression target)
        {
            Type = type;
            Subject = subject;
            Target = target;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.InGroup;

        public override Type Type { get; }

        internal Expression Subject { get; private set; }
        internal Expression Target { get; private set; }
    }
}

