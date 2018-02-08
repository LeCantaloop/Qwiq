using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class OrderExpression : Expression
    {
        internal OrderExpression(Type type, Expression source, Expression orderSelector, OrderOptions options)
        {
            Type = type;
            Source = source;
            OrderSelector = orderSelector;
            Options = options;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.Order;

        public override Type Type { get; }

        internal Expression Source { get; private set; }

        internal Expression OrderSelector { get; private set; }

        internal OrderOptions Options { get; private set; }
    }
}
