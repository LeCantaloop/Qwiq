using Qwiq.Credentials;

namespace Qwiq.Mocks
{
    public class MockWorkItemStoreFactory : WorkItemStoreFactory
    {
        public override IWorkItemStore Create(AuthenticationOptions options)
        {
            return new MockWorkItemStore();
        }
    }
}
