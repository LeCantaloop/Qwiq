using System.Collections;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    public interface IFieldCollection : IEnumerable, IEnumerable<IField>
    {
        IField this[string name] { get; }
        int Count { get; }
        bool Contains(string fieldName);
    }
}
