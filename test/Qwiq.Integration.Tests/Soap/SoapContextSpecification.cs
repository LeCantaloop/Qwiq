using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qwiq.Tests.Common;
using System;
using SoapWorkItemStore = Qwiq.Client.Soap.WorkItemStore;
using IInternalTeamProjectCollection = Qwiq.Client.Soap.IInternalTeamProjectCollection;

namespace Qwiq.Soap
{
    /// <summary>
    /// Base class for SOAP client unit tests using Moq.
    /// Provides common infrastructure for testing SOAP WorkItemStore without TFS connectivity.
    /// </summary>
    /// <remarks>
    /// These tests exercise the SOAP client adapter logic by mocking:
    /// - IInternalTeamProjectCollection (Qwiq interface)
    /// - IQueryFactory (Qwiq interface) 
    /// - IQuery (Qwiq interface)
    ///
    /// The TFS Client OM (Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemStore) is NOT mocked.
    /// Instead, we inject mock implementations via factory functions.
    /// </remarks>
    public abstract class SoapContextSpecification : ContextSpecification
    {
        private Mock<IInternalTeamProjectCollection>? _mockTeamProjectCollection;

        protected Mock<IQueryFactory>? MockQueryFactory { get; set; }
        protected IWorkItemStore? Store { get; set; }

        public override void Given()
        {
            // Create mocks
            _mockTeamProjectCollection = new Mock<IInternalTeamProjectCollection>(MockBehavior.Strict);
            MockQueryFactory = new Mock<IQueryFactory>(MockBehavior.Strict);

            // Setup basic TPC properties
            _mockTeamProjectCollection
                .Setup(x => x.Dispose())
                .Verifiable();

            // Create WorkItemStore with mocked dependencies
            // Note: We pass a factory that throws for the TFS WorkItemStore since we'll mock at the IQueryFactory level
            Store = new SoapWorkItemStore(
                () => _mockTeamProjectCollection.Object,
                () => throw new NotSupportedException("TFS WorkItemStore should not be accessed in unit tests"),
                _ => MockQueryFactory.Object);
        }

        public override void Cleanup()
        {
            Store?.Dispose();
            base.Cleanup();
        }
    }
}
