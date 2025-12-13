using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using System;
using WireMock.Server;

namespace Qwiq.WireMock
{
    /// <summary>
    /// Base class for REST client tests that use WireMock for HTTP mocking.
    /// </summary>
    /// <remarks>
    /// This base class provides a WireMock server and helper methods for configuring
    /// mock responses. Tests derived from this class exercise the full REST client
    /// implementation without requiring live Azure DevOps connectivity.
    /// </remarks>
    [TestCategory("WireMock")]
#pragma warning disable CA1001 // Disposable field '_context' is disposed in Cleanup() method
    public abstract class WireMockRestContextSpecification : TimedContextSpecification
#pragma warning restore CA1001
    {
        private WireMockRestStoreContext? _context;

        /// <summary>
        /// Gets the WireMock server for configuring mock responses.
        /// </summary>
        protected WireMockServer Server => _context?.Server ?? throw new InvalidOperationException("Context not initialized.");

        /// <summary>
        /// Gets the work item store backed by WireMock.
        /// </summary>
        protected IWorkItemStore? Store { get; private set; }

        /// <summary>
        /// Gets the base URL of the WireMock server.
        /// </summary>
        protected string BaseUrl => _context?.BaseUrl ?? throw new InvalidOperationException("Context not initialized.");

        /// <summary>
        /// Initializes the WireMock context using real captured Azure DevOps API responses.
        /// </summary>
        public override void Given()
        {
            _context = new WireMockRestStoreContext();

            System.Diagnostics.Trace.WriteLine($"WireMock server started at: {Server.Url}");

            // Load real Azure DevOps API responses from captured stubs
            // The captured stubs include the connectionData response with full service definitions
            // and resource locations, so we don't need to call SetupVssConnectionHandshake
            var stubsPath = AzureDevOpsWireMockExtensions.GetDefaultStubsFilePath();
            Server.LoadStubsFromFile(stubsPath);

            System.Diagnostics.Trace.WriteLine($"WireMock has {Server.Mappings.Count} registered mappings");

            // Create the store after stubs are loaded
            Store = TimedAction(() => _context.CreateWorkItemStore(), "WireMock", "Create WorkItemStore");
        }

        public override void Cleanup()
        {
            (Store as IDisposable)?.Dispose();
            _context?.Dispose();
            base.Cleanup();
        }
    }
}
