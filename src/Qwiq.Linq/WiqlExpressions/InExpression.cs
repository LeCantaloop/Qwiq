using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class InExpression : Expression
    {
        internal InExpression(Type type, Expression subject, Expression target)
        {
            Type = type;
            Subject = subject;
            Target = target;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.In;

        public override Type Type { get; }

        internal Expression Subject { get; private set; }
        internal Expression Target { get; private set; }
    }
}

