using System;

using Qwiq.Exceptions;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Client.Soap
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ITeamProjectCollection" />
    internal class TfsTeamProjectCollection : IInternalTeamProjectCollection
    {
        private readonly Lazy<ICommonStructureService> _css;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TfsTeamProjectCollection" /> class.
        /// </summary>
        /// <param name="teamProjectCollection">The TFS.</param>
        /// <exception cref="ArgumentNullException">tfs</exception>
        internal TfsTeamProjectCollection(Microsoft.TeamFoundation.Client.TfsTeamProjectCollection teamProjectCollection)
        {
            Native = teamProjectCollection ?? throw new ArgumentNullException(nameof(teamProjectCollection));

            AuthorizedCredentials = Native.ClientCredentials;
            AuthorizedIdentity = new TeamFoundationIdentity(Native.AuthorizedIdentity);
            Uri = Native.Uri;

            _css = new Lazy<ICommonStructureService>(
                                                     () => ExceptionHandlingDynamicProxyFactory.Create<ICommonStructureService>(
                                                                                                                                new
                                                                                                                                        CommonStructureService(
                                                                                                                                                               Native
                                                                                                                                                                       .GetService
                                                                                                                                                                       <
                                                                                                                                                                           ICommonStructureService4
                                                                                                                                                                       >())));
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
        public bool HasAuthenticated => Native.HasAuthenticated;

        /// <summary>
        ///     This is used to convert dates and times to UTC.
        /// </summary>
        /// <value>The time zone.</value>
        public TimeZone TimeZone => Native.TimeZone;

        /// <summary>
        ///     The base url for this connection
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri { get; }

        internal Microsoft.TeamFoundation.Client.TfsTeamProjectCollection Native { get; }

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
            return Native.GetClient<T>();
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T GetService<T>()
        {
            return Native.GetService<T>();
        }

        public override string ToString()
        {
            return Native.Name;
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
            if (disposing) Native?.Dispose();
        }
    }
}