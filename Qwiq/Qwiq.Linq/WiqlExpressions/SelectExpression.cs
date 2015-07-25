using System;
using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Linq.WiqlExpressions
{
    public class SelectExpression : Expression
    {
        private readonly Type _type;

        public Expression Select { get; private set; }

        public LambdaExpression Projection { get; private set; }

        internal SelectExpression(Type type, Expression select, LambdaExpression projection)
        {
            _type = type;
            Select = select;
            Projection = projection;
        }

        public override Type Type
        {
            get { return _type; }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)WiqlExpressionType.Select;
            }
        }
    }
}
