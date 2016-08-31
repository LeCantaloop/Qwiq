using System;
using System.Collections.Generic;

using Microsoft.IE.Qwiq.Credentials;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.IE.Qwiq.Proxies;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(Uri endpoint, TfsCredentials credentials);

        IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials);
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
            return Create(endpoint, new [] { credentials });
        }

        public IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials)
        {
            Func<WorkItemStore, IQueryFactory> queryFactoryFunc = QueryFactory.GetInstance;
            foreach (var credential in credentials)
            {
                try
                {
                    var tfsNative = ConnectToTfsCollection(endpoint, credential.Credentials);

                    System.Diagnostics.Trace.TraceInformation("TFS connection attempt success with {0}/{1}.", credential.Credentials.Windows.GetType(), credential.Credentials.Federated.GetType());

                    var tfs = ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(new TfsTeamProjectCollectionProxy(tfsNative));
                    var workItemStore = tfs.GetService<WorkItemStore>();
                    var queryFactory = queryFactoryFunc.Invoke(workItemStore);

                    return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemStore>(new WorkItemStoreProxy(tfs, workItemStore, queryFactory));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceWarning("TFS connection attempt failed with {0}/{1}.\n Exception: {2}", credential.Credentials.Windows.GetType(), credential.Credentials.Federated.GetType(), e);
                }
            }

            System.Diagnostics.Trace.TraceError("All TFS connection attempts failed.");
            throw new AccessDeniedException("Invalid credentials");
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
