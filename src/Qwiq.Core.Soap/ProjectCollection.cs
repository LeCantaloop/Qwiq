using System.Linq;

using Qwiq.Exceptions;

namespace Qwiq.Client.Soap
{
    internal class ProjectCollection : Qwiq.ProjectCollection
    {
        internal ProjectCollection(Microsoft.TeamFoundation.WorkItemTracking.Client.ProjectCollection valueProjects)
            : base(
                   valueProjects.Cast<Microsoft.TeamFoundation.WorkItemTracking.Client.Project>()
                                .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IProject>(new Project(item))).ToList()
                                )
        {
        }
    }
}