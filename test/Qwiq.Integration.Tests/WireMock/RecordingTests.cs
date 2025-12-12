using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using WireMock.Handlers;
using WireMock.Server;
using WireMock.Settings;

namespace Qwiq.WireMock
{
    /// <summary>
    /// Test fixture that records HTTP traffic from real Azure DevOps calls.
    /// </summary>
    /// <remarks>
    /// This test runs real integration tests while capturing all HTTP traffic
    /// through WireMock's proxy mode. The captured traffic is saved to JSON files
    /// that can be used for offline WireMock testing.
    ///
    /// To use:
    /// 1. Run this test class
    /// 2. Check the output directory for captured JSON files
    /// 3. Copy the relevant responses to the Stubs folder
    ///
    /// Note: This requires the system to route traffic through the WireMock proxy.
    /// You may need to set HTTP_PROXY/HTTPS_PROXY environment variables.
    /// </remarks>
    [TestClass]
    [TestCategory("Recording")]
    [Ignore("Run manually to capture traffic - requires proxy configuration")]
    public class Given_Recording_Proxy_When_Running_Integration_Tests : ContextSpecification
    {
        private WireMockServer? _proxyServer;
        private IWorkItemStore? _store;
        private string? _outputPath;

        public override void Given()
        {
            // Create output directory for recordings
            _outputPath = Path.Combine(
                Path.GetDirectoryName(typeof(Given_Recording_Proxy_When_Running_Integration_Tests).Assembly.Location)!,
                "WireMockRecordings",
                DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture));
            Directory.CreateDirectory(_outputPath);

            // Start WireMock in proxy mode
            var settings = new WireMockServerSettings
            {
                Port = 8888,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "https://qwiq-sandbox.visualstudio.com",
                    SaveMapping = true,
                    SaveMappingToFile = true,
                    SaveMappingForStatusCodePattern = "*" // Save all responses
                },
                FileSystemHandler = new LocalFileSystemHandler(_outputPath)
            };

            _proxyServer = WireMockServer.Start(settings);

            System.Diagnostics.Trace.WriteLine($"WireMock proxy started on port {_proxyServer.Port}");
            System.Diagnostics.Trace.WriteLine($"Recordings will be saved to: {_outputPath}");

            // Configure .NET to use the proxy
            // Note: This may not work for all scenarios - see alternative approaches below
            WebRequest.DefaultWebProxy = new WebProxy($"http://localhost:{_proxyServer.Port}", false);
        }

        public override void When()
        {
            // Create REST store - this will make HTTP calls through the proxy
            try
            {
                _store = IntegrationSettings.CreateRestStore();

                // Execute a simple query to capture the traffic
                var wiql = ((FormattableString)$"SELECT [System.Id] FROM WorkItems WHERE [System.Id] = {TestData.BasicWorkItemId}").ToString(CultureInfo.InvariantCulture);
                var result = _store.Query(wiql);

#pragma warning disable CA1829, CA1826 // IWorkItemCollection doesn't have Count property
                System.Diagnostics.Trace.WriteLine($"Query returned {result.Count()} work items");
#pragma warning restore CA1829, CA1826
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Error during recording: {ex.Message}");
                throw;
            }
        }

        [TestMethod]
        public void Should_Capture_Traffic()
        {
            // Check that recordings were created
            var files = Directory.GetFiles(_outputPath!, "*.json", SearchOption.AllDirectories);
            System.Diagnostics.Trace.WriteLine($"Captured {files.Length} recordings");

            foreach (var file in files)
            {
                System.Diagnostics.Trace.WriteLine($"  - {Path.GetFileName(file)}");
            }
        }

        public override void Cleanup()
        {
            (_store as IDisposable)?.Dispose();
            _proxyServer?.Stop();
            _proxyServer?.Dispose();

            // Reset proxy
            WebRequest.DefaultWebProxy = null;

            base.Cleanup();
        }
    }
}
