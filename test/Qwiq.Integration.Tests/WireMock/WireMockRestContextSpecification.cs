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
    ///
    /// Use this for:
    /// - Unit testing REST client behavior
    /// - Testing error handling scenarios
    /// - Testing edge cases that are hard to reproduce with real data
    /// - Fast, isolated tests that don't require network access
    ///
    /// For integration tests against real Azure DevOps, use <see cref="TimedContextSpecification"/>
    /// with <see cref="IntegrationSettings.CreateRestStore"/>.
    ///
    /// Note: WireMock requires HTTPS because VssBasicCredential enforces secure connections.
    /// On CI runners where HTTPS startup fails (due to SSL certificate binding privileges),
    /// tests are automatically marked as inconclusive rather than failing.
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
        protected WireMockServer Server => _context?.Server ?? throw new InvalidOperationException("Context not initialized. Call base.Given() first.");

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
        /// <remarks>
        /// If WireMock HTTPS startup fails (e.g., on CI runners without SSL certificate binding privileges),
        /// the test will be marked as inconclusive rather than failing.
        /// </remarks>
        public override void Given()
        {
            try
            {
                _context = new WireMockRestStoreContext();
            }
            catch (WireMockHttpsStartupException ex)
            {
                // Mark test as inconclusive when HTTPS startup fails
                // This is expected on CI runners without SSL certificate binding privileges
                System.Diagnostics.Trace.TraceWarning(
                    "WireMock HTTPS startup failed. Test will be marked as inconclusive. " +
                    "This is expected on CI runners without SSL certificate binding privileges. " +
                    "Error: {0}", ex.Message);

                Assert.Inconclusive(
                    "WireMock HTTPS server could not start. This test requires HTTPS which needs " +
                    "elevated privileges for SSL certificate binding. This is expected on CI runners " +
                    "(e.g., GitHub Actions) where such privileges are restricted. " +
                    "Run this test locally with administrator privileges. " +
                    "Inner error: " + ex.InnerException?.Message);
            }

            // Load real Azure DevOps API responses from captured stubs
            // This includes VssConnection handshake, projects, WIQL queries, work items, and work item types
            var stubsPath = AzureDevOpsWireMockExtensions.GetDefaultStubsFilePath();
            Server.LoadStubsFromFile(stubsPath);

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
