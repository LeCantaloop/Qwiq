using System;

using Qwiq.WorkItemStore;

namespace Qwiq.Project
{
    public abstract class ProjectComparisonContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        private Guid projectId;

        protected IProject RestProject { get; private set; }

        protected IProject SoapProject { get; private set; }

        public override void Given()
        {
            projectId = IntegrationSettings.ProjectGuid;
            base.Given();
        }

        public override void When()
        {
            RestProject = TimedAction(() => Rest.Projects[projectId], "REST", "Get Project by Guid");
            SoapProject = TimedAction(() => Soap.Projects[projectId], "SOAP", "Get Project by Guid");
        }
    }
}