using System.Collections.Generic;
using System.Linq;


namespace Qwiq.Mocks
{
    internal class MockProjectCollection : ProjectCollection
    {
        public MockProjectCollection(IWorkItemStore store)
            : this(new MockProject(store))
        {
        }

        public MockProjectCollection(MockProject project)
            : this(new[] { (IProject)project }.ToList())
        {
        }

        public MockProjectCollection(List<IProject> projects)
            : base(projects)
        {
        }
    }
}