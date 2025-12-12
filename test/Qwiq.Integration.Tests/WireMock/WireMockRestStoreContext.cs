using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Qwiq.Client.Rest;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using WireMock.Server;
using WireMock.Settings;

namespace Qwiq.WireMock
{
    /// <summary>
    /// Exception thrown when WireMock HTTPS server fails to start due to SSL certificate binding issues.
    /// </summary>
    /// <remarks>
    /// This exception is thrown when WireMock cannot start an HTTPS server, typically because:
    /// - The process lacks elevated privileges to register SSL certificate bindings
    /// - The SSL certificate cannot be created or bound to the port
    /// - This commonly occurs on CI runners (e.g., GitHub Actions) where elevated privileges are restricted
    ///
    /// Tests should catch this exception and skip/inconclusive the test rather than failing.
    /// </remarks>
    public class WireMockHttpsStartupException : Exception
    {
        public WireMockHttpsStartupException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Provides a WireMock-based REST store context for testing the REST client implementation
    /// without requiring live Azure DevOps connectivity.
    /// </summary>
    /// <remarks>
    /// This context creates a WireMock server with HTTPS and configures a VssConnection to use it.
    /// Tests can configure mock responses using the <see cref="Server"/> property.
    ///
    /// The WireMock server intercepts HTTP traffic, allowing full coverage of the REST client
    /// implementation including serialization, HTTP handling, and error scenarios.
    ///
    /// Note: HTTPS is required because VssBasicCredential enforces secure connections.
    /// On CI runners where HTTPS startup fails (due to SSL certificate binding privileges),
    /// a <see cref="WireMockHttpsStartupException"/> is thrown so tests can be skipped gracefully.
    /// </remarks>
    public class WireMockRestStoreContext : IDisposable
    {
        private bool _disposed;
        private readonly RemoteCertificateValidationCallback? _originalCallback;

        /// <summary>
        /// Gets the WireMock server instance for configuring mock responses.
        /// </summary>
        public WireMockServer Server { get; }

        /// <summary>
        /// Gets the base URL of the WireMock server (HTTPS).
        /// </summary>
        public string BaseUrl => Server.Url!;

        /// <summary>
        /// Gets the VssConnection configured to use the WireMock server.
        /// </summary>
        public VssConnection Connection { get; }

        /// <summary>
        /// Gets the WorkItemTrackingHttpClient configured to use the WireMock server.
        /// This is lazily initialized when first accessed to allow mock responses to be configured first.
        /// </summary>
        public WorkItemTrackingHttpClient WorkItemTrackingClient => _workItemTrackingClient ??= Connection.GetClient<WorkItemTrackingHttpClient>();
        private WorkItemTrackingHttpClient? _workItemTrackingClient;

        /// <summary>
        /// Creates a new WireMock-based REST store context with HTTPS.
        /// </summary>
        /// <remarks>
        /// WireMock uses a self-signed certificate for HTTPS. This context temporarily
        /// bypasses certificate validation during testing.
        ///
        /// IMPORTANT: Configure mock responses using the Server property BEFORE
        /// accessing WorkItemTrackingClient or calling CreateWorkItemStore().
        /// The VssConnection.GetClient&lt;T&gt;() call requires the connection data
        /// endpoint to be mocked first.
        ///
        /// Note: HTTPS is required because VssBasicCredential enforces secure connections.
        /// If HTTPS startup fails (e.g., on CI runners without SSL certificate binding privileges),
        /// a <see cref="WireMockHttpsStartupException"/> is thrown so tests can be skipped gracefully.
        /// </remarks>
        /// <exception cref="WireMockHttpsStartupException">
        /// Thrown when WireMock cannot start an HTTPS server, typically due to SSL certificate binding issues.
        /// </exception>
        public WireMockRestStoreContext()
        {
            // Save original certificate validation callback
            _originalCallback = ServicePointManager.ServerCertificateValidationCallback;

            // Bypass SSL certificate validation for WireMock's self-signed cert
            // This is safe for testing purposes only
#pragma warning disable CA5359 // Intentionally accepting all certificates for WireMock testing
            ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertificates;
#pragma warning restore CA5359

            // Start WireMock server with HTTPS (required for VssBasicCredential)
            // VssBasicCredential enforces "Basic authentication requires a secure connection to the server"
            var settings = new WireMockServerSettings
            {
                UseSSL = true,
                Port = null // Use random available port
            };

            try
            {
                Server = WireMockServer.Start(settings);
            }
            catch (Exception ex) when (IsSslBindingFailure(ex))
            {
                // Restore original callback before throwing
                ServicePointManager.ServerCertificateValidationCallback = _originalCallback;

                throw new WireMockHttpsStartupException(
                    "WireMock HTTPS server failed to start. This typically occurs on CI runners " +
                    "(e.g., GitHub Actions) where elevated privileges for SSL certificate binding are restricted. " +
                    "Tests using WireMock should be skipped in this environment.",
                    ex);
            }

            // Create VssConnection pointing to WireMock with HTTPS
            // Use basic credentials (empty) - WireMock doesn't validate auth
            var credentials = new VssBasicCredential(string.Empty, string.Empty);

            // Create connection with settings that work for local testing
            Connection = new VssConnection(new Uri(BaseUrl), credentials);
            Connection.Settings.BypassProxyOnLocal = true;
            Connection.Settings.CompressionEnabled = false; // Easier debugging

            // NOTE: Do NOT call Connection.GetClient<T>() here!
            // The VssConnection requires the /_apis/connectionData endpoint to be mocked first.
            // The WorkItemTrackingClient property is lazily initialized when first accessed.
        }

        /// <summary>
        /// Determines if an exception indicates an SSL certificate binding failure.
        /// </summary>
        private static bool IsSslBindingFailure(Exception ex)
        {
            // Check for common SSL binding failure patterns
            // AggregateException wraps the actual HttpListenerException
            if (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(IsSslBindingFailure))
            {
                return true;
            }

            // HttpListenerException with error code 5 (Access Denied) or 183 (Cannot create file)
            // indicates SSL certificate binding issues
            var message = ex.Message?.ToUpperInvariant() ?? string.Empty;
            return message.Contains("ACCESS") ||
                   message.Contains("DENIED") ||
                   message.Contains("CERTIFICATE") ||
                   message.Contains("SSL") ||
                   message.Contains("SERVICE START FAILED");
        }

        /// <summary>
        /// Creates an IWorkItemStore using the WireMock-backed connection.
        /// </summary>
        /// <returns>A work item store that sends requests to WireMock.</returns>
        public IWorkItemStore CreateWorkItemStore()
        {
            // Use the internal constructor that accepts a custom client factory
            var tpcProxy = Connection.AsProxy();
            if (tpcProxy == null)
            {
                throw new InvalidOperationException("Failed to create TPC proxy from VssConnection.");
            }

            // Cast to IInternalTeamProjectCollection which provides GetClient<T>
            var internalTpc = (IInternalTeamProjectCollection)tpcProxy;

            return new Client.Rest.WorkItemStore(
                () => internalTpc,
                () => WorkItemTrackingClient,
                QueryFactory.GetInstance);
        }

        private static bool AcceptAllCertificates(
            object sender,
            X509Certificate? certificate,
            X509Chain? chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // Accept all certificates for WireMock testing
            return true;
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
                // Restore original certificate validation callback
                ServicePointManager.ServerCertificateValidationCallback = _originalCallback;

                _workItemTrackingClient?.Dispose();
                Connection?.Dispose();
                Server?.Stop();
                Server?.Dispose();
            }

            _disposed = true;
        }
    }
}
