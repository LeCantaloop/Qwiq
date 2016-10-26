using System;

namespace Microsoft.Qwiq.Relatives.WiqlExpressions
{
    internal class ChildrenExpression : RelativesExpression
    {
        public ChildrenExpression(Type type, Type parentType, Type childType)
            : base(type, parentType, childType, RelativesQueryDirection.DescendentFromAncestor)
        {
        }
    }
}

