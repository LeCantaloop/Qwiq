using System;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Soap
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ITeamProjectCollection" />
    internal class TfsTeamProjectCollection : IInternalTeamProjectCollection
    {
        private readonly Lazy<ICommonStructureService> _css;

        private readonly Lazy<IIdentityManagementService> _ims;

        private readonly TeamFoundation.Client.TfsTeamProjectCollection _tfs;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TfsTeamProjectCollection" /> class.
        /// </summary>
        /// <param name="teamProjectCollection">The TFS.</param>
        /// <exception cref="ArgumentNullException">tfs</exception>
        internal TfsTeamProjectCollection(TeamFoundation.Client.TfsTeamProjectCollection teamProjectCollection)
        {
            _tfs = teamProjectCollection ?? throw new ArgumentNullException(nameof(teamProjectCollection));

            AuthorizedCredentials = _tfs.ClientCredentials;
            AuthorizedIdentity = new TeamFoundationIdentity(_tfs.AuthorizedIdentity);
            Uri = _tfs.Uri;

            _css = new Lazy<ICommonStructureService>(
                () => ExceptionHandlingDynamicProxyFactory.Create<ICommonStructureService>(
                    new CommonStructureService(_tfs.GetService<ICommonStructureService4>())));
            _ims = new Lazy<IIdentityManagementService>(
                () => ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(
                    new IdentityManagementService(_tfs.GetService<IIdentityManagementService2>())));
        }

        /// <summary>
        ///     Gets the credentials for this project collection.
        /// </summary>
        /// <value>The authorized credentials.</value>
        public VssCredentials AuthorizedCredentials { get; }

        /// <summary>
        ///     The identity who the calls to the server are being made for.
        /// </summary>
        /// <value>The authorized identity.</value>
        public ITeamFoundationIdentity AuthorizedIdentity { get; }

        /// <summary>
        ///     Gets the common structure service.
        /// </summary>
        /// <value>The common structure service.</value>
        public ICommonStructureService CommonStructureService => _css.Value;

        /// <summary>
        ///     Returns true if this object has successfully authenticated.
        /// </summary>
        /// <value><c>true</c> if this instance has authenticated; otherwise, <c>false</c>.</value>
        public bool HasAuthenticated => _tfs.HasAuthenticated;

        /// <summary>
        ///     Gets the identity management service.
        /// </summary>
        /// <value>The identity management service.</value>
        public IIdentityManagementService IdentityManagementService => _ims.Value;

        /// <summary>
        ///     This is used to convert dates and times to UTC.
        /// </summary>
        /// <value>The time zone.</value>
        public TimeZone TimeZone => _tfs.TimeZone;

        /// <summary>
        ///     The base url for this connection
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets the client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T GetClient<T>()
        {
            return _tfs.GetClient<T>();
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T GetService<T>()
        {
            return _tfs.GetService<T>();
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) _tfs?.Dispose();
        }

        public override string ToString()
        {
            return _tfs.Name;
        }
    }
}