using System;
using System.Globalization;

namespace Qwiq
{
    public class QueryFolder : IQueryFolder
    {
        public QueryFolder(Guid id, string name, IQueryFolderCollection subFolders, IQueryDefinitionCollection queries)
        {
            Id = id;
            Name = name;
            SubFolders = subFolders;
            SavedQueries = queries;
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
            FormattableString s = $"{Id.ToString()} ({Name})";
            return s.ToString(CultureInfo.InvariantCulture);
        }
    }
}