using System;
using Microsoft.IE.Qwiq.Credentials;

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
            return new WorkItemStoreProxy(endpoint, credentials);
        }
    }
}
