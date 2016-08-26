using System;
using Microsoft.IE.Qwiq.Credentials;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.IE.Qwiq.Proxies;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(Uri endpoint, TfsCredentials credentials);
    }

    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        private static readonly Lazy<WorkItemStoreFactory> Instance = new Lazy<WorkItemStoreFactory>(() => new WorkItemStoreFactory());

        private WorkItemStoreFactory()
        {
        }

        public static IWorkItemStoreFactory GetInstance()
        {
            return Instance.Value;
        }

        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials)
        {
            Func<WorkItemStore, IQueryFactory> queryFactoryFunc = QueryFactory.GetInstance;
            return Create(endpoint, credentials, queryFactoryFunc);
        }

        internal IWorkItemStore Create(Uri endpoint, TfsCredentials credentials,
            Func<WorkItemStore, IQueryFactory> queryFactoryFunc)
        {
            var tfs = ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(new TfsTeamProjectCollectionProxy(ConnectToTfsCollection(endpoint, credentials.Credentials)));
            var workItemStore = tfs.GetService<WorkItemStore>();
            var queryFactory = queryFactoryFunc.Invoke(workItemStore);

            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemStore>(new WorkItemStoreProxy(tfs, workItemStore, queryFactory));
        }

        private static TfsTeamProjectCollection ConnectToTfsCollection(
            Uri endpoint,
            TfsClientCredentials credentials)
        {
            var tfsServer = new TfsTeamProjectCollection(endpoint, credentials);
            tfsServer.EnsureAuthenticated();
            return tfsServer;
        }
    }
}
