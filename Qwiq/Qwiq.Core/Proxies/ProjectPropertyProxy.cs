using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.Qwiq.Proxies
{
    public class ProjectPropertyProxy : IProjectProperty
    {
        private readonly Tfs.ProjectProperty _projectProperty;

        internal ProjectPropertyProxy(Tfs.ProjectProperty projectProperty)
        {
            _projectProperty = projectProperty;
        }
    }
}

