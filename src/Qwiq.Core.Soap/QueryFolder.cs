using System.Linq;
using JetBrains.Annotations;
using Qwiq.Exceptions;

namespace Qwiq.Client.Soap
{
    internal class QueryFolder : Qwiq.QueryFolder
    {
        internal QueryFolder([NotNull] Microsoft.TeamFoundation.WorkItemTracking.Client.QueryFolder queryFolder)
            : base(
                queryFolder.Id,
                queryFolder.Name,
                queryFolder.Path,
                new QueryFolderCollection(() =>
                {
                    return queryFolder.OfType<Microsoft.TeamFoundation.WorkItemTracking.Client.QueryFolder>()
                        .Select(qf => ExceptionHandlingDynamicProxyFactory.Create<IQueryFolder>(new QueryFolder(qf)));
                }),
                new QueryDefinitionCollection(() =>
                {
                    return queryFolder.OfType<Microsoft.TeamFoundation.WorkItemTracking.Client.QueryDefinition>()
                        .Select(qd => ExceptionHandlingDynamicProxyFactory.Create<IQueryDefinition>(new QueryDefinition(qd)));
                }))
        {
        }
    }
}
