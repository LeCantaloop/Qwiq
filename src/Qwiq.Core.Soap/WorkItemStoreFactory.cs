using System;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq.Client.Soap
{
    public class WorkItemStoreFactory : Qwiq.WorkItemStoreFactory
    {
        public static readonly IWorkItemStoreFactory Default = Nested.Instance;

        private WorkItemStoreFactory()
        {
        }

        [Obsolete("This method is deprecated and will be removed in a future release. See property Default instead.", false)]
        public static IWorkItemStoreFactory GetInstance()
        {
            return Default;
        }

        public override IWorkItemStore Create(AuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var tfsProxy = (IInternalTeamProjectCollection)TfsConnectionFactory.Default.Create(options);
            return CreateSoapWorkItemStore(tfsProxy);
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