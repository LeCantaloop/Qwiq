using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IFieldCollection : IReadOnlyCollection<IField>
    {
        IField this[string name] { get; }
        bool Contains(string name);

        IField TryGetById(int id);

        IField GetById(int id);
    }
}

