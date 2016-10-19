using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Core.Tests
{
    public interface IContextSpecification
    {
        [TestInitialize]
        void TestInitialize();

        [TestCleanup]
        void TestCleanup();

        void Given();

        void When();

        void Cleanup();
    }
}
