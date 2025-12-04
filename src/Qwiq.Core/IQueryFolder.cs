using System;

namespace Qwiq
{
    public interface IQueryFolder : IIdentifiable<Guid>, IEquatable<IQueryFolder>
    {
        string Name { get; }
        string Path { get; }
        IQueryFolderCollection SubFolders { get; }
        IQueryDefinitionCollection SavedQueries { get; }
    }
}