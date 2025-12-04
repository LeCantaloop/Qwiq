using System;

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Qwiq.Client.Rest
{
    internal class VssConnectionAdapter : IInternalTeamProjectCollection
    {
        private readonly VssConnection _connection;

        internal VssConnectionAdapter(VssConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            AuthorizedCredentials = connection.Credentials;
            AuthorizedIdentity = new TeamFoundationIdentity(_connection.AuthorizedIdentity);
            HasAuthenticated = connection.HasAuthenticated;
            Uri = connection.Uri;
            TimeZone = TimeZone.CurrentTimeZone;
        }

        public VssCredentials AuthorizedCredentials { get; }

        public ITeamFoundationIdentity AuthorizedIdentity { get; }

        /// <summary>
        /// Gets the common structure service. Not supported in REST API.
        /// </summary>
        /// <exception cref="NotSupportedException">REST API does not support CommonStructureService.</exception>
        public ICommonStructureService CommonStructureService =>
            throw new NotSupportedException("CommonStructureService is not available via the REST API. Use SOAP client for this functionality.");

        public bool HasAuthenticated { get; }

        public TimeZone TimeZone { get; }

        public Uri Uri { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T GetClient<T>()
            where T : VssHttpClientBase
        {
            return _connection.GetClient<T>();
        }

        public T GetService<T>()
            where T : IVssClientService
        {
            return _connection.GetService<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _connection.Disconnect();
        }
    }
}