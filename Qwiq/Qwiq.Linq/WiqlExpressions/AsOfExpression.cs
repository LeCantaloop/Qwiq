using System;
using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq.WiqlExpressions
{
    public class AsOfExpression : Expression
    {
        private readonly Type _type;

        internal AsOfExpression(Type type, DateTime asOfDateTime)
        {
            _type = type;

            AsOfDateTime = asOfDateTime;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)WiqlExpressionType.AsOf;
            }
        }
        public override Type Type
        {
            get
            {
                return _type;
            }
        }

        internal DateTime AsOfDateTime { get; private set; }
    }
}
