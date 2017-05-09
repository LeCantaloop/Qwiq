using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemStoreFactory : WorkItemStoreFactory
    {
        public override IWorkItemStore Create(AuthenticationOptions options)
        {
            return new MockWorkItemStore();
        }
    }
}
