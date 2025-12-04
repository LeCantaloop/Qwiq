using System;

namespace Qwiq.Mocks
{
    public class MockQueryFolder : QueryFolder
    {
        public MockQueryFolder(Guid id, string name, string path, IQueryFolderCollection subFolders, IQueryDefinitionCollection queries)
            : base(id, name, path, subFolders, queries)
        {
        }
    }
}
