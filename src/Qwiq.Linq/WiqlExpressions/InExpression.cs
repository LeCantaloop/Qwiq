using System;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.WiqlExpressions
{
    public class InExpression : Expression
    {
        private readonly Type _type;

        internal InExpression(Type type, Expression subject, Expression target)
        {
            _type = type;

            Subject = subject;
            Target = target;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)WiqlExpressionType.In;
            }
        }
        public override Type Type
        {
            get
            {
                return _type;
            }
        }

        internal Expression Subject { get; private set; }
        internal Expression Target { get; private set; }
    }
}

