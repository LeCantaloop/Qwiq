using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Rest
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
            if (options.ClientType != ClientType.Rest) throw new InvalidOperationException();

            var tfsProxy = (IInternalTeamProjectCollection)TfsConnectionFactory.Default.Create(options);
            var wis = CreateRestWorkItemStore(tfsProxy);
            return ExceptionHandlingDynamicProxyFactory.Create(wis);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials, ClientType type = ClientType.Default)
        {
            return Create(endpoint, new[] { credentials }, type);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(
            Uri endpoint,
            IEnumerable<TfsCredentials> credentials,
            ClientType type = ClientType.Default)
        {
            var options = new AuthenticationOptions(endpoint, AuthenticationTypes.Windows, type, types => credentials.Select(s=>s.Credentials));
            return Create(options);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See property Default instead.",
            false)]
        public static IWorkItemStoreFactory GetInstance()
        {
            return Default;
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