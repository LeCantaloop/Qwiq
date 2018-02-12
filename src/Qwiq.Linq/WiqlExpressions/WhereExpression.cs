using System;
using System.Linq.Expressions;

namespace Qwiq.Linq.WiqlExpressions
{
    public class WhereExpression : Expression
    {
        internal WhereExpression(Type type, Expression source, Expression filter)
        {
            Type = type;
            Source = source;
            Filter = filter;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.Where;

        public override Type Type { get; }

        internal Expression Source { get; private set; }

        internal Expression Filter { get; private set; }
    }
}
