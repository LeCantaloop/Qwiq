using System;

namespace Microsoft.IE.Qwiq.Relatives.WiqlExpressions
{
    internal class ParentsExpression : RelativesExpression
    {
        public ParentsExpression(Type type, Type parentType, Type childType) : base(type, parentType, childType, RelativesQueryDirection.AncestorFromDescendent)
        {
        }
    }
}
