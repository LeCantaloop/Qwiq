using System;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.WiqlExpressions
{
    public class UnderExpression : Expression
    {
        internal UnderExpression(Type type, Expression subject, Expression target)
        {
            Type = type;
            Subject = subject;
            Target = target;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.Under;

        public override Type Type { get; }

        internal Expression Subject { get; private set; }
        internal Expression Target { get; private set; }
    }
}

