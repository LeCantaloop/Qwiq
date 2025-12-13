using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Qwiq.Client.Rest;
using System;
using WireMock.Server;
using WireMock.Settings;

namespace Qwiq.WireMock
{
    /// <summary>
    /// Provides a WireMock-based REST store context for testing the REST client implementation
    /// without requiring live Azure DevOps connectivity.
    /// </summary>
    /// <remarks>
    /// This context creates a WireMock HTTP server and configures a VssConnection to use it.
    /// Uses HTTP because VssCredentials() allows HTTP for on-premises scenarios.
    /// </remarks>
    public class WireMockRestStoreContext : IDisposable
    {
        private bool _disposed;
        private WorkItemTrackingHttpClient? _workItemTrackingClient;

        /// <summary>
        /// Gets the WireMock server instance for configuring mock responses.
        /// </summary>
        public WireMockServer Server { get; }

        /// <summary>
        /// Gets the base URL of the WireMock server (HTTP).
        /// </summary>
        public string BaseUrl => Server.Url!;

        /// <summary>
        /// Gets the VssConnection configured to use the WireMock server.
        /// </summary>
        public VssConnection Connection { get; }

        /// <summary>
        /// Gets the WorkItemTrackingHttpClient configured to use the WireMock server.
        /// </summary>
        public WorkItemTrackingHttpClient WorkItemTrackingClient =>
            _workItemTrackingClient ??= Connection.GetClient<WorkItemTrackingHttpClient>();

        /// <summary>
        /// Creates a new WireMock-based REST store context with HTTP.
        /// </summary>
        public WireMockRestStoreContext()
        {
            var settings = new WireMockServerSettings
            {
                Urls = new[] { "http://127.0.0.1:0" },
                StartAdminInterface = true,
                AllowPartialMapping = true
            };
            Server = WireMockServer.Start(settings);

            System.Diagnostics.Trace.WriteLine($"[WireMock] Server started at: {BaseUrl}");

            var credentials = new VssCredentials();
            Connection = new VssConnection(new Uri(BaseUrl), credentials);
            Connection.Settings.BypassProxyOnLocal = true;
            Connection.Settings.CompressionEnabled = false;
            Connection.Settings.SendTimeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Creates an IWorkItemStore using the WireMock-backed connection.
        /// </summary>
        public IWorkItemStore CreateWorkItemStore()
        {
            var tpcProxy = Connection.AsProxy();
            if (tpcProxy == null)
            {
                throw new InvalidOperationException("Failed to create TPC proxy from VssConnection.");
            }

            var internalTpc = (IInternalTeamProjectCollection)tpcProxy;

            return new Client.Rest.WorkItemStore(
                () => internalTpc,
                () => WorkItemTrackingClient,
                QueryFactory.GetInstance);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (Server != null)
                {
                    System.Diagnostics.Trace.WriteLine($"[WireMock] Total mappings: {Server.Mappings.Count}");
                    System.Diagnostics.Trace.WriteLine("[WireMock] Requests received:");
                    foreach (var entry in Server.LogEntries)
                    {
                        var matched = entry.MappingGuid != null && entry.MappingGuid != Guid.Empty;
                        var status = matched ? "MATCHED" : "UNMATCHED";
                        System.Diagnostics.Trace.WriteLine($"  [{status}] {entry.RequestMessage.Method} {entry.RequestMessage.Url}");
                    }
                }

                _workItemTrackingClient?.Dispose();
                Connection?.Dispose();
                Server?.Stop();
                Server?.Dispose();
            }

            _disposed = true;
        }
    }
}
