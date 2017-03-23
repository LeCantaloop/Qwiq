using System;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class WorkItemTypeProxy : IWorkItemType
    {
        private readonly Tfs.WorkItemType _type;

        internal WorkItemTypeProxy(Tfs.WorkItemType type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Description => _type.Description;

        public IFieldDefinitionCollection FieldDefinitions => ExceptionHandlingDynamicProxyFactory
            .Create<IFieldDefinitionCollection>(new FieldDefinitionCollectionProxy(_type.FieldDefinitions));

        public string Name => _type.Name;

        public IWorkItem NewWorkItem()
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(_type.NewWorkItem()));
        }
    }
}