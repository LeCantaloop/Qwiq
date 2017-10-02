using System;

namespace Qwiq
{
    public interface IQueryFolderCollection : IReadOnlyObjectWithIdCollection<IQueryFolder, Guid>, IEquatable<IQueryFolderCollection>
    {
    }
}