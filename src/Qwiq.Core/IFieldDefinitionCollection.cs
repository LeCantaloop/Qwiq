using System;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinitionCollection : IReadOnlyCollectionWithId<IFieldDefinition, int>,
                                                  IEquatable<IFieldDefinitionCollection>
    {
    }
}