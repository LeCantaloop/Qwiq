using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Qwiq.Mocks
{
    internal class MockProjectCollection : ProjectCollection
    {
        public MockProjectCollection([NotNull] IWorkItemStore store)
            : this(new MockProject(store))
        {
        }

        public MockProjectCollection([NotNull] MockProject project)
            : this(new[] { (IProject)project }.ToList())
        {
        }

        public MockProjectCollection([NotNull] List<IProject> projects)
            : base(projects)
        {
        }
    }
}