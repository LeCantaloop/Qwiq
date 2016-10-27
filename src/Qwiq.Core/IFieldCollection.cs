using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IFieldCollection : IEnumerable<IField>
    {
        IField this[string name] { get; }
        int Count { get; }
        bool Contains(string fieldName);
    }
}

