using Qwiq.Credentials;

namespace Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(AuthenticationOptions options);
    }
}