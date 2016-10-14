using System;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.WiqlExpressions
{
    public class OrderExpression : Expression
    {
        private readonly Type _type;

        internal OrderExpression(Type type, Expression source, Expression orderSelector, OrderOptions options)
        {
            _type = type;

            Source = source;
            OrderSelector = orderSelector;
            Options = options;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)WiqlExpressionType.Order;
            }
        }

        public override Type Type
        {
            get
            {
                return _type;
            }
        }

        internal Expression Source { get; private set; }

        internal Expression OrderSelector { get; private set; }

        internal OrderOptions Options { get; private set; }
    }
}
