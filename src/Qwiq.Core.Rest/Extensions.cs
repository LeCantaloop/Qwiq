using Microsoft.Qwiq.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Qwiq.Client.Rest
{
    internal static class Extensions
    {
        internal static IWorkItem AsProxy(this WorkItem item)
        {
            return item == null
                ? null
                : ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(item);
        }

        internal static IQuery AsProxy(this Query query)
        {
            return query == null
                ? null
                : ExceptionHandlingDynamicProxyFactory.Create<IQuery>(query);
        }

        internal static IIdentityDescriptor AsProxy(this VisualStudio.Services.Identity.IdentityDescriptor value)
        {
            return value == null
                ? null
                : ExceptionHandlingDynamicProxyFactory.Create<IIdentityDescriptor>(new IdentityDescriptor(value));
        }

        internal static IInternalTeamProjectCollection AsProxy(this VssConnection tfsNative)
        {
            return tfsNative == null
                       ? null
                       : ExceptionHandlingDynamicProxyFactory.Create<IInternalTeamProjectCollection>(new VssConnectionAdapter(tfsNative));
        }
    }
}
