using System;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class ProjectComparisonContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected IProject RestProject { get; private set; }
        protected IProject SoapProject { get; private set; }
        private Guid projectId;
        public override void Given()
        {
            projectId = Guid.Parse("8d47e068-03c8-4cdc-aa9b-fc6929290322");
            base.Given();
        }

        public override void When()
        {
            RestProject = TimedAction(()=> Rest.Projects[projectId], "REST", "Get Project by Guid");
            SoapProject = TimedAction(() => Soap.Projects[projectId], "SOAP", "Get Project by Guid");
        }
    }
}