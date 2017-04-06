using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    internal class Project : Qwiq.Project
    {
        internal Project(TeamProjectReference project, WorkItemStore store)
            : base(
                project.Id,
                project.Name,
                new Uri(project.Url),
                new Lazy<IWorkItemTypeCollection>(
                    () =>
                        {
                            var wits = store.NativeWorkItemStore.Value.GetWorkItemTypesAsync(project.Name).GetAwaiter().GetResult();
                            return new WorkItemTypeCollection(wits.Select(s => new WorkItemType(s)));
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