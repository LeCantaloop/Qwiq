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
            throw new NotImplementedException();
        }
    }
}