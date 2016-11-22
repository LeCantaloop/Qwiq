using System;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.WiqlExpressions
{
    public class AsOfExpression : Expression
    {
        internal AsOfExpression(Type type, DateTime asOfDateTime)
        {
            Type = type;
            AsOfDateTime = asOfDateTime;
        }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.AsOf;

        public override Type Type { get; }

        internal DateTime AsOfDateTime { get; private set; }
    }
}

