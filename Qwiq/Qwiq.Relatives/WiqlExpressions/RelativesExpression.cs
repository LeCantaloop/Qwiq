using System;
using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Relatives.WiqlExpressions
{
    public enum RelativesQueryDirection
    {
        AncestorFromDescendent,
        DescendentFromAncestor
    }

    public abstract class RelativesExpression : Expression
    {
        private readonly Type _type;
        public Type AncestorType { get; private set; }
        public Type DescendentType { get; private set; }

        public RelativesQueryDirection Direction { get; private set; }

        protected RelativesExpression(Type type, Type ancestorType, Type descendentType, RelativesQueryDirection direction)
        {
            _type = type;
            AncestorType = ancestorType;
            DescendentType = descendentType;
            Direction = direction;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)WiqlExpressionType.Relatives; }
        }

        public override Type Type
        {
            get { return _type; }
        }
    }
}
