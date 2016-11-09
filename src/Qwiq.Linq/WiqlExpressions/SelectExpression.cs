using System;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.WiqlExpressions
{
    public class SelectExpression : Expression
    {
        public Expression Select { get; private set; }

        public LambdaExpression Projection { get; private set; }

        internal SelectExpression(Type type, Expression select, LambdaExpression projection)
        {
            Type = type;
            Select = select;
            Projection = projection;
        }

        public override Type Type { get; }

        public override ExpressionType NodeType => (ExpressionType)WiqlExpressionType.Select;
    }
}

