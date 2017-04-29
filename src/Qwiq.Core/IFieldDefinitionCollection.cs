using System;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinitionCollection : IReadOnlyObjectWithIdList<IFieldDefinition, int>,
                                                  IEquatable<IFieldDefinitionCollection>
    {
    }
}