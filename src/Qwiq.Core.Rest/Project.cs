using System;
using System.Linq;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
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
                            var wits = store.NativeWorkItemStore
                                            .Value
                                            .GetWorkItemTypesAsync(project.Name)
                                            .GetAwaiter()
                                            .GetResult();
                            return new WorkItemTypeCollection(wits.Select(s => new WorkItemType(s)));
                        }),
                new Lazy<INodeCollection>(
                    () =>
                        {
                            var result = store.NativeWorkItemStore
                                              .Value
                                              .GetClassificationNodeAsync(
                                                  project.Id,
                                                  TreeStructureGroup.Areas,
                                                  null,
                                                  int.MaxValue)
                                              .GetAwaiter()
                                              .GetResult();

                            // SOAP Client does not return just the root, so return the root's children to match implementation
                            return new NodeCollection(new Node(result).ChildNodes);
                        }),
                new Lazy<INodeCollection>(
                    () =>
                        {
                            var result = store.NativeWorkItemStore
                                              .Value
                                              .GetClassificationNodeAsync(
                                                  project.Name,
                                                  TreeStructureGroup.Iterations,
                                                  null,
                                                  int.MaxValue)
                                              .GetAwaiter()
                                              .GetResult();

                            return new NodeCollection(new Node(result).ChildNodes);
                        }))
        {
        }
    }
}