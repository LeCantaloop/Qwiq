using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemTypeProxy : IWorkItemType
    {
        internal WorkItemTypeProxy(WorkItemType type)
        {
            Description = type.Description;
            Name = type.Name;
            FieldDefinitions = new FieldDefinitionCollectionProxy(type.Fields);
        }

        public string Description { get; }

        public IFieldDefinitionCollection FieldDefinitions { get; }

        public string Name { get; }

        public IWorkItem NewWorkItem()
        {
            throw new NotSupportedException();

            /*
             * In order to support this, we need:
             *  1) an instance of WorkItemTrackingHttpClient (or WorkItemStoreProxy),
             *  2) the name of the project (or IProject)
             *
             * Creating an item then looks like this
             *
             * var patch = new JsonPatchDocument();
             * patch.Add(
             *      new JsonPatchOperation()
             *      {
             *          Operation = Operation.Add,
             *          Path = "/fields/System.Title",
             *          Value = "Some Title"
             *      }
             * );
             *
             * var result = WorkItemStoreProxy.NativeClient.CreateWorkItemAsync(patch, projectName, Name).GetAwaiter().GetResult();
             * return new WorkItemProxy(result);
             *
             */
        }
    }
}