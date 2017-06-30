using Microsoft.Qwiq.WorkItemStore;

namespace Microsoft.Qwiq.Project
{
    public abstract class ProjectContextSpecificationSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected IProjectCollection RestProjects { get; set; }

        protected IProjectCollection SoapProjects { get; set; }

        public override void When()
        {
            RestProjects = Rest.Projects;
            SoapProjects = Soap.Projects;
        }
    }
}