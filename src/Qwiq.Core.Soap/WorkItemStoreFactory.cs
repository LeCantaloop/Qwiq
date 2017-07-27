using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Client.Soap
{
    public class WorkItemStoreFactory : Qwiq.WorkItemStoreFactory
    {
        public static readonly IWorkItemStoreFactory Default = Nested.Instance;

        private WorkItemStoreFactory()
        {
        }

        public override IWorkItemStore Create(AuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var tfsProxy = (IInternalTeamProjectCollection)TfsConnectionFactory.Default.Create(options);
            return ExceptionHandlingDynamicProxyFactory.Create(CreateSoapWorkItemStore(tfsProxy));
        }

        private static IWorkItemStore CreateSoapWorkItemStore(IInternalTeamProjectCollection tfs)
        {
            return new WorkItemStore(() => tfs, store => new QueryFactory(store));
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
                // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemStoreFactory Instance = new WorkItemStoreFactory();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}