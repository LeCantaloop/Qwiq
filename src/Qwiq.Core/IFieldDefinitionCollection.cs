using System;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinitionCollection : IReadOnlyListWithId<IFieldDefinition, int>,
                                                  IEquatable<IFieldDefinitionCollection>
    {
    }
}