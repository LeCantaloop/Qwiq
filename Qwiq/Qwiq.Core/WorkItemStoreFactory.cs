using System;
using Microsoft.IE.Qwiq.Proxies;
using Microsoft.IE.Qwiq.Credentials;
using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(Uri endpoint, TfsCredentials credentials);
    }

    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        private WorkItemStoreFactory()
        {
        }

        public static IWorkItemStoreFactory GetInstance()
        {
            return new WorkItemStoreFactory();
        }

        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials)
        {
            Func<Tfs.WorkItemTracking.Client.WorkItemStore, IQueryFactory> queryFactoryFunc = QueryFactory.GetInstance;
            return Create(endpoint,credentials, queryFactoryFunc);
        }

        internal IWorkItemStore Create(Uri endpoint, TfsCredentials credentials,
            Func<Tfs.WorkItemTracking.Client.WorkItemStore, IQueryFactory> queryFactoryFunc)
        {
            var tfs = ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(new TfsTeamProjectCollectionProxy(new Tfs.Client.TfsTeamProjectCollection(endpoint, credentials.Credentials)));
            var workItemStore = tfs.GetService<Tfs.WorkItemTracking.Client.WorkItemStore>();
            var queryFactory = queryFactoryFunc.Invoke(workItemStore);

            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemStore>(new WorkItemStoreProxy(tfs, workItemStore, queryFactory));
        }
    }
}
