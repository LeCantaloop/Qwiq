using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public abstract class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        /// <inheritdoc />
        public abstract IWorkItemStore Create(AuthenticationOptions options);
    }
}