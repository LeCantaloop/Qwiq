using Qwiq.Credentials;

namespace Qwiq
{
    public abstract class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        /// <inheritdoc />
        public abstract IWorkItemStore Create(AuthenticationOptions options);
    }
}