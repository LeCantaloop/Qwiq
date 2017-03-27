using System;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Soap;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    public partial class WorkItemTypeProxy
    {
        internal WorkItemTypeProxy(Tfs.WorkItemType type)
            : this(
                 type?.Name,
                 type?.Description,
                 new Lazy<IFieldDefinitionCollection>(() => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinitionCollection>(new FieldDefinitionCollectionProxy(type?.FieldDefinitions))),
                 () => ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(type?.NewWorkItem()))
                 )
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
        }
    }
}