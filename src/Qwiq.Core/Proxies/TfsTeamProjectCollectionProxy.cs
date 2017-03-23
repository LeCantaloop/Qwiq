using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Soap;


using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.Qwiq.Proxies
{
    public class TfsTeamProjectCollectionProxy : IInternalTfsTeamProjectCollection
    {
        private readonly Lazy<ICommonStructureService> _css;

        private readonly Lazy<IIdentityManagementService> _ims;

        private readonly Tfs.Client.TfsTeamProjectCollection _tfs;

        internal TfsTeamProjectCollectionProxy(Tfs.Client.TfsTeamProjectCollection tfs)
        {
            _tfs = tfs;
            _css =
                new Lazy<ICommonStructureService>(
                    () =>
                        ExceptionHandlingDynamicProxyFactory.Create<ICommonStructureService>(
                            new CommonStructureServiceProxy(_tfs?.GetService<Tfs.Server.ICommonStructureService4>())));
            _ims =
                new Lazy<IIdentityManagementService>(
                    () =>
                        ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(
                            new IdentityManagementServiceProxy(
                                _tfs?.GetService<Tfs.Framework.Client.IIdentityManagementService2>())));
        }

        public TfsCredentials AuthorizedCredentials => new TfsCredentials(_tfs?.ClientCredentials);

        public ITeamFoundationIdentity AuthorizedIdentity => new TeamFoundationIdentityProxy(_tfs?.AuthorizedIdentity);

        public ICommonStructureService CommonStructureService => _css.Value;

        public bool HasAuthenticated => _tfs?.HasAuthenticated ?? false;

        public IIdentityManagementService IdentityManagementService => _ims.Value;

        public TimeZone TimeZone => _tfs?.TimeZone ?? TimeZone.CurrentTimeZone;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T GetService<T>()
        {
            return _tfs == null ? default(T) : _tfs.GetService<T>();
        }

        public T GetClient<T>()
        {
            return _tfs.GetClient<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tfs?.Dispose();
            }
        }
    }
}