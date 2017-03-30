using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.Qwiq.Soap
{
    internal class ProjectProperty : IProjectProperty
    {
        private readonly Tfs.ProjectProperty _projectProperty;

        internal ProjectProperty(Tfs.ProjectProperty projectProperty)
        {
            _projectProperty = projectProperty;
        }
    }
}

