using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qwiq.Tests.Common
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
