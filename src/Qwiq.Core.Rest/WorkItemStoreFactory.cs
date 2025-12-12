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
            return Create(options, TfsConnectionFactory.Default);
        }

        /// <summary>
        /// Creates a new <see cref="IWorkItemStore"/> instance with the specified authentication options and connection factory.
        /// </summary>
        /// <param name="options">The authentication options.</param>
        /// <param name="connectionFactory">The connection factory to use for creating TFS connections. Allows test injection.</param>
        /// <returns>A new <see cref="IWorkItemStore"/> instance.</returns>
        /// <remarks>
        /// This overload is internal to enable unit testing without requiring live Azure DevOps connectivity.
        /// Production code should use the public <see cref="Create(AuthenticationOptions)"/> overload.
        /// </remarks>
        internal IWorkItemStore Create(AuthenticationOptions options, ITfsConnectionFactory connectionFactory)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(connectionFactory);

            var tfsProxy = (IInternalTeamProjectCollection)connectionFactory.Create(options);
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