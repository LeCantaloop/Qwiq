using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinitionCollection : IEnumerable<IFieldDefinition>
    {
        int Count { get; }
        IFieldDefinition this[string name] { get; }

        bool Contains(string fieldName);
    }
}