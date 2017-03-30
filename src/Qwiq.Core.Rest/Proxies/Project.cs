using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class Project : Qwiq.Project
    {
        internal Project(TeamProjectReference project, WorkItemStoreProxy store)
            : base(
                // REST API stores ID as GUID rather than INT
                // Converting from 128-bit GUID will have some loss in precision
                BitConverter.ToInt32(project.Id.ToByteArray(), 0),
                project.Id,
                project.Name,
                new Uri(project.Url),
                new Lazy<IEnumerable<IWorkItemType>>(
                    () =>
                        {
                            var wits = store.NativeWorkItemStore.Value.GetWorkItemTypesAsync(project.Name).GetAwaiter().GetResult();
                            return wits.Select(s => new WorkItemType(s));
                        }),
                new Lazy<IEnumerable<INode>>(
                    () =>
                        {
                            var result = store.NativeWorkItemStore.Value
                                              .GetClassificationNodeAsync(project.Name, TreeStructureGroup.Areas)
                                              .GetAwaiter()
                                              .GetResult();

                            return new[] { new Node(result), };
                        }),
                new Lazy<IEnumerable<INode>>(
                    () =>
                        {
                            var result = store.NativeWorkItemStore.Value
                                              .GetClassificationNodeAsync(project.Name, TreeStructureGroup.Iterations)
                                              .GetAwaiter()
                                              .GetResult();

                            return new[] { new Node(result) };
                        })
                 )
        {
        }
    }
}