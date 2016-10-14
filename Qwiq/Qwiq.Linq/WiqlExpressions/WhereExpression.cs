using System;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.WiqlExpressions
{
    public class WhereExpression : Expression
    {
        private readonly Type _type;

        internal WhereExpression(Type type, Expression source, Expression filter)
        {
            _type = type;

            Source = source;
            Filter = filter;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)WiqlExpressionType.Where;
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

        internal Expression Filter { get; private set; }
    }
}
