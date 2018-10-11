using System;
using JetBrains.Annotations;

namespace Qwiq.Mocks
{
    public class MockQueryFolder : QueryFolder
    {
        public MockQueryFolder(Guid id, [NotNull] string name, [NotNull] string path, [NotNull] IQueryFolderCollection subFolders, [NotNull] IQueryDefinitionCollection queries)
            : base(id, name, path, subFolders, queries)
        {
        }
    }
}
