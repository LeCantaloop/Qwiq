using System;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class WorkItemType : Qwiq.WorkItemType
    {
        internal WorkItemType(Tfs.WorkItemType type)
            : base(
                 type?.Name,
                 type?.Description,
                 new Lazy<IFieldDefinitionCollection>(() => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinitionCollection>(new FieldDefinitionCollection(type?.FieldDefinitions))),
                 () => ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(type?.NewWorkItem()))
                 )
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
        }
    }
}