using System;
using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mocks
{
    public class MockQueryFolder : QueryFolder
    {
        public MockQueryFolder(Guid id, [NotNull] string name, [NotNull] IQueryFolderCollection subFolders, [NotNull] IQueryDefinitionCollection queries)
            : base(id, name, subFolders, queries)
        {
        }
    }
}
