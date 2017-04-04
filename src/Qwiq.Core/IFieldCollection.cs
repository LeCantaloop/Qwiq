using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IFieldCollection : IReadOnlyCollection<IField>
    {
        IField this[string name] { get; }
        bool Contains(string name);

        bool TryGetById(int id, out IField field);

        IField GetById(int id);
    }
}

