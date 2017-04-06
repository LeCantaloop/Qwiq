using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class ProjectContextSpecificationSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected List<IProject> RestProjects { get; set; }

        protected List<IProject> SoapProjects { get; set; }



        public override void When()
        {
            RestProjects = Rest.Projects.ToList();
            SoapProjects = Soap.Projects.ToList();
        }
    }
}