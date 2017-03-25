using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class ProjectProxy : IProject
    {
        private readonly Lazy<IEnumerable<IWorkItemType>> _workItemTypes;

        private readonly Lazy<IEnumerable<INode>> _area;
        private readonly Lazy<IEnumerable<INode>> _iteration;

        internal ProjectProxy(TeamProjectReference project, WorkItemStoreProxy store)
        {
            // REST API stores ID as GUID rather than INT
            // Converting from 128-bit GUID will have some loss in precision
            Id = BitConverter.ToInt32(project.Id.ToByteArray(), 0);
            Name = project.Name;
            Uri = new Uri(project.Url);
            Store = store;

            _workItemTypes = new Lazy<IEnumerable<IWorkItemType>>(
                () =>
                    {
                        var wits = store.NativeWorkItemStore.Value.GetWorkItemTypesAsync(Name).GetAwaiter().GetResult();
                        return wits.Select(s => new WorkItemTypeProxy(s));
                    });

            _area = new Lazy<IEnumerable<INode>>(
                () =>
                    {
                        var result = store.NativeWorkItemStore.Value
                                          .GetClassificationNodeAsync(Name, TreeStructureGroup.Areas)
                                          .GetAwaiter()
                                          .GetResult();

                        return new[] { new WorkItemClassificationNodeProxy(result), };

                    });

            _iteration = new Lazy<IEnumerable<INode>>(
                () =>
                    {
                        var result = store.NativeWorkItemStore.Value
                                          .GetClassificationNodeAsync(Name, TreeStructureGroup.Iterations)
                                          .GetAwaiter()
                                          .GetResult();

                        return new[] { new WorkItemClassificationNodeProxy(result), };

                    });
        }

        public IEnumerable<INode> AreaRootNodes => _area.Value;

        public int Id { get; }

        public IEnumerable<INode> IterationRootNodes => _iteration.Value;

        public string Name { get; }

        public Uri Uri { get; }

        public IEnumerable<IWorkItemType> WorkItemTypes => _workItemTypes.Value;

        public IWorkItemStore Store { get; }
    }
}