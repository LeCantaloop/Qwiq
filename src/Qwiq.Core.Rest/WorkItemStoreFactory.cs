using System;

using Qwiq.Credentials;
using Qwiq.Exceptions;

namespace Qwiq.Client.Rest
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
            var wis = CreateRestWorkItemStore(tfsProxy);
            return ExceptionHandlingDynamicProxyFactory.Create(wis);
        }

        private static IWorkItemStore CreateRestWorkItemStore(IInternalTeamProjectCollection tfs)
        {
            return new WorkItemStore(() => tfs, QueryFactory.GetInstance);
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