using System;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinitionCollection : IReadOnlyObjectWithIdCollection<IFieldDefinition, int>,
                                                  IEquatable<IFieldDefinitionCollection>
    {
    }
}