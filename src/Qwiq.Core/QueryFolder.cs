using System;
using System.Globalization;

using JetBrains.Annotations;

namespace Qwiq
{
    public abstract class QueryFolder : IQueryFolder
    {
        internal QueryFolder(Guid id, [NotNull] string name, [NotNull] string path, [NotNull] IQueryFolderCollection subFolders, [NotNull] IQueryDefinitionCollection queries)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            Id = id;
            Name = name;
            Path = path;
            SubFolders = subFolders ?? throw new ArgumentNullException(nameof(subFolders));
            SavedQueries = queries ?? throw new ArgumentNullException(nameof(queries));
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Path { get; set; }

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
            FormattableString s = $"{Id} ({Name})";
            return s.ToString(CultureInfo.InvariantCulture);
        }
    }
}
