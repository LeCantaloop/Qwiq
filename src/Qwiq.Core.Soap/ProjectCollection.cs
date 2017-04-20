using Microsoft.Qwiq.Exceptions;

using System.Linq;

namespace Microsoft.Qwiq.Soap
{
    internal class ProjectCollection : Qwiq.ProjectCollection
    {


        public ProjectCollection(TeamFoundation.WorkItemTracking.Client.ProjectCollection valueProjects)
            :base(valueProjects.Cast<TeamFoundation.WorkItemTracking.Client.Project>()
                                .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IProject>(new Project(item))))
        {
        }



    }
}