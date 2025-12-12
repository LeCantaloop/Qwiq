using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Qwiq.Credentials;
using System;

namespace Qwiq.Client.Rest
{
    /// <summary>
    /// Mock implementation of ITfsConnectionFactory for unit testing REST client without Azure DevOps connectivity.
    /// </summary>
    internal class MockTfsConnectionFactory : ITfsConnectionFactory
    {
        private readonly Uri _baseUri;
        private readonly Func<WorkItemTrackingHttpClient>? _httpClientFactory;

        /// <summary>
        /// Creates a new mock connection factory that redirects to the specified base URI (typically a WireMock server).
        /// </summary>
        /// <param name="baseUri">The base URI to use for connections (e.g., WireMock server URL).</param>
        /// <param name="httpClientFactory">Optional factory for creating WorkItemTrackingHttpClient. If null, uses Moq.</param>
        public MockTfsConnectionFactory(Uri baseUri, Func<WorkItemTrackingHttpClient>? httpClientFactory = null)
        {
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _httpClientFactory = httpClientFactory;
        }

        public ITeamProjectCollection Create(AuthenticationOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            // Create a mock connection that redirects to WireMock server
            var mockConnection = CreateMockConnection();
            return mockConnection.AsProxy() ?? throw new InvalidOperationException("Failed to create proxy.");
        }

        private VssConnection CreateMockConnection()
        {
            // Create VssConnection with basic credentials
            var credentials = new VssBasicCredential(string.Empty, string.Empty);
            var connection = new VssConnection(_baseUri, credentials);

            // Configure for testing
            connection.Settings.BypassProxyOnLocal = true;
            connection.Settings.CompressionEnabled = false; // Disable compression for easier debugging

            return connection;
        }
    }
}
