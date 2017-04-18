using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(IAuthenticationOptions options);
    }
}