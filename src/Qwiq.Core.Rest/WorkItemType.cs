using System;
using System.Diagnostics.Contracts;


namespace Qwiq.Client.Rest
{
    internal class WorkItemType : Qwiq.WorkItemType
    {
        private readonly Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType _type;
        private IFieldDefinitionCollection? _fdc;

        internal WorkItemType(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType type)
            : base(
                type.Name,
                type.Description,
                null!,
                NewWorkItemImpl)
        {
            Contract.Requires(type != null);
            Contract.Requires(type != null);

            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        /// <inheritdoc />
        public override IFieldDefinitionCollection FieldDefinitions => _fdc ?? (_fdc = new FieldDefinitionCollection(_type.Fields));

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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_type != null);
        }
    }
}