using System;
using System.Collections.Generic;

namespace Qwiq
{
    public class QueryFolderCollection : ReadOnlyObjectWithIdCollection<IQueryFolder, Guid>, IQueryFolderCollection
    {
        internal QueryFolderCollection(Func<IEnumerable<IQueryFolder>> queryFolderFactory)
            : base(queryFolderFactory, folder => folder.Name)
        {
        }

        public override bool Equals(object obj)
        {
            return Comparer.QueryFolderCollection.Equals(this, obj as IQueryFolderCollection);
        }

        public bool Equals(IQueryFolderCollection other)
        {
            return Comparer.QueryFolderCollection.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Comparer.QueryFolderCollection.GetHashCode(this);
        }
    }
}