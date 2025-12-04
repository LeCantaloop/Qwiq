
using Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Qwiq.Client.Rest
{
    internal static class Extensions
    {

        internal static IWorkItem AsProxy(this WorkItem item)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(item);
        }

        internal static IQuery AsProxy(this Query query)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(query);
        }

        internal static IIdentityDescriptor? AsProxy(this Microsoft.VisualStudio.Services.Identity.IdentityDescriptor? value)
        {
            return value == null ? null : ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new IdentityDescriptor(value));
        }

        internal static IInternalTeamProjectCollection? AsProxy(this VssConnection? tfsNative)
        {
            return tfsNative == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IInternalTeamProjectCollection>(new VssConnectionAdapter(tfsNative));
        }
        internal static bool IsFolder(this QueryHierarchyItem item)
        {
            return (item?.IsFolder != null) && item.IsFolder.Value;
        }
        internal static bool IsExpanded(this QueryHierarchyItem item)
        {
            return (item?.HasChildren != null) && item.HasChildren.Value && (item.Children != null);
        }
    }
}
