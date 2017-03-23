using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemTypeProxy : IWorkItemType
    {
        private readonly WorkItemType _type;

        internal WorkItemTypeProxy(WorkItemType type)
        {
            _type = type;
        }

        public string Description => _type.Description;

        public IFieldDefinitionCollection FieldDefinitions => new FieldDefinitionCollectionProxy(_type.Fields);

        public string Name => _type.Name;

        public IWorkItem NewWorkItem()
        {
            throw new NotImplementedException();
        }
    }
}