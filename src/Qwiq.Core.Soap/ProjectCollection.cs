using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class ProjectCollection : Qwiq.ProjectCollection
    {
        internal ProjectCollection(TeamFoundation.WorkItemTracking.Client.ProjectCollection valueProjects)
            : base(
                   valueProjects.Cast<TeamFoundation.WorkItemTracking.Client.Project>()
                                .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IProject>(new Project(item))).ToList()
                                )
        {
        }
    }
}