using System;

namespace Qwiq
{
    public interface IFieldDefinitionCollection : IReadOnlyObjectWithIdCollection<IFieldDefinition, int>,
                                                  IEquatable<IFieldDefinitionCollection>
    {
    }
}