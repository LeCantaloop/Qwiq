using System;
using System.Globalization;

using JetBrains.Annotations;

namespace Qwiq
{
    public class QueryFolder : IQueryFolder
    {
        protected QueryFolder(Guid id, [NotNull] string name, [NotNull] IQueryFolderCollection subFolders, [NotNull] IQueryDefinitionCollection queries)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            Id = id;
            Name = name;
            SubFolders = subFolders ?? throw new ArgumentNullException(nameof(subFolders));
            SavedQueries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public Guid Id { get; }
        public string Name { get; }

        public IQueryFolderCollection SubFolders { get; }

        public IQueryDefinitionCollection SavedQueries { get; }

        public override bool Equals(object obj)
        {
            return QueryFolderComparer.Default.Equals(this, obj as IQueryFolder);
        }

        public bool Equals(IQueryFolder other)
        {
            return QueryFolderComparer.Default.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return QueryFolderComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            return $"{Id} ({Name})".ToString(CultureInfo.InvariantCulture);
        }
    }
}
