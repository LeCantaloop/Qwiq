using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class WorkItemTypeProxy : Microsoft.Qwiq.Proxies.WorkItemTypeProxy
    {
        internal WorkItemTypeProxy(WorkItemType type)
            : base(
                type?.Name,
                type?.Description,
                new Lazy<IFieldDefinitionCollection>(() => new FieldDefinitionCollectionProxy(type?.Fields)),
                NewWorkItemImpl)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
        }

        private static IWorkItem NewWorkItemImpl()
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