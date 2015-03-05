namespace Microsoft.IE.Qwiq
{
    public class ProjectPropertyProxy : IProjectProperty
    {
        private readonly TeamFoundation.Server.ProjectProperty _projectProperty;

        public ProjectPropertyProxy(TeamFoundation.Server.ProjectProperty projectProperty)
        {
            _projectProperty = projectProperty;
        }
    }
}
