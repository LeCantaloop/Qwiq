using Qwiq.WorkItemStore;

namespace Qwiq.Project
{
    public abstract class ProjectContextSpecificationSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected IProjectCollection RestProjects { get; set; } = null!;

        protected IProjectCollection SoapProjects { get; set; } = null!;

        public override void When()
        {
            RestProjects = Rest.Projects;
            SoapProjects = Soap.Projects;
        }
    }
}