using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq.Client.Soap
{
    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        public static readonly IWorkItemStoreFactory Default = Nested.Instance;

        private WorkItemStoreFactory()
        {
        }

        public IWorkItemStore Create(AuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var tfsProxy = (IInternalTeamProjectCollection)TfsConnectionFactory.Default.Create(options);
            return CreateSoapWorkItemStore(tfsProxy);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials)
        {
            return Create(endpoint, new[] { credentials });
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(
            Uri endpoint,
            IEnumerable<TfsCredentials> credentials)
        {
            var options = new AuthenticationOptions(endpoint, AuthenticationTypes.Windows, types => credentials.Select(s=>s.Credentials));
            return Create(options);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See property Default instead.",
            false)]
        public static IWorkItemStoreFactory GetInstance()
        {
            return Default;
        }

        private static IWorkItemStore CreateSoapWorkItemStore(IInternalTeamProjectCollection tfs)
        {
            return new WorkItemStore(() => tfs, store =>  new QueryFactory(store));
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