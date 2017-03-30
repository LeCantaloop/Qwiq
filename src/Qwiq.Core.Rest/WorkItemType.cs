using System;

namespace Microsoft.Qwiq.Rest
{
    internal class WorkItemType : Qwiq.WorkItemType
    {
        internal WorkItemType(TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType type)
            : base(
                type?.Name,
                type?.Description,
                new Lazy<IFieldDefinitionCollection>(() => new FieldDefinitionCollection(type?.Fields)),
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