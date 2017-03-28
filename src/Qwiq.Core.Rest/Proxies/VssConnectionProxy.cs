using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Qwiq.Rest.Proxies
{
    public class VssConnectionProxy : IInternalTfsTeamProjectCollection
    {
        private readonly VssConnection _connection;

        public VssConnectionProxy(VssConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            AuthorizedCredentials = new TfsCredentials(connection.Credentials);
            AuthorizedIdentity = new TeamFoundationIdentityProxy(_connection.AuthorizedIdentity);
            HasAuthenticated = connection.HasAuthenticated;
            Uri = connection.Uri;
            TimeZone = TimeZone.CurrentTimeZone;
        }

        public TfsCredentials AuthorizedCredentials { get; }

        public ITeamFoundationIdentity AuthorizedIdentity { get; }

        public ICommonStructureService CommonStructureService { get; }

        public bool HasAuthenticated { get; }

        public IIdentityManagementService IdentityManagementService { get; }

        public TimeZone TimeZone { get; }

        public Uri Uri { get; }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}