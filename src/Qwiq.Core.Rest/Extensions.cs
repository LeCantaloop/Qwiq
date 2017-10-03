using JetBrains.Annotations;

using Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Qwiq.Client.Rest
{
    internal static class Extensions
    {
        [CanBeNull]
        [Pure]
        [ContractAnnotation("null => null; notnull => notnull")]
        internal static IWorkItem AsProxy([CanBeNull] this WorkItem item)
        {
            return item == null ? null : ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(item);
        }

        [CanBeNull]
        [Pure]
        [ContractAnnotation("null => null; notnull => notnull")]
        internal static IQuery AsProxy([CanBeNull] this Query query)
        {
            return query == null ? null : ExceptionHandlingDynamicProxyFactory.Create<IQuery>(query);
        }

        [CanBeNull]
        [Pure]
        [ContractAnnotation("null => null; notnull => notnull")]
        internal static IIdentityDescriptor AsProxy([CanBeNull] this Microsoft.VisualStudio.Services.Identity.IdentityDescriptor value)
        {
            return value == null ? null : ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new IdentityDescriptor(value));
        }

        [CanBeNull]
        [Pure]
        [ContractAnnotation("null => null; notnull => notnull")]
        internal static IInternalTeamProjectCollection AsProxy([CanBeNull] this VssConnection tfsNative)
        {
            return tfsNative == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IInternalTeamProjectCollection>(new VssConnectionAdapter(tfsNative));
        }

        internal static bool IsFolder([NotNull] this QueryHierarchyItem item)
        {
            return (item.IsFolder != null) && item.IsFolder.Value;
        }

        internal static bool IsExpanded([NotNull] this QueryHierarchyItem item)
        {
            return item.HasChildren.HasValue && item.HasChildren.Value && (item.Children != null);
        }
    }
}
