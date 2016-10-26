using System;
using Microsoft.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.Qwiq.Proxies
{
    public class TfsTeamProjectCollectionProxy : IInternalTfsTeamProjectCollection
    {
        private readonly Tfs.Client.TfsTeamProjectCollection _tfs;

        internal TfsTeamProjectCollectionProxy(Tfs.Client.TfsTeamProjectCollection tfs)
        {
            _tfs = tfs;
        }

        public IIdentityManagementService IdentityManagementService
        {
            get
            {
                return ExceptionHandlingDynamicProxyFactory.Create<IIdentityManagementService>(new IdentityManagementServiceProxy(_tfs.GetService<Tfs.Framework.Client.IIdentityManagementService2>()));
            }
        }

        public ICommonStructureService CommonStructureService
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<ICommonStructureService>(new CommonStructureServiceProxy(_tfs.GetService<Tfs.Server.ICommonStructureService4>())); }
        }

        public T GetService<T>()
        {
            return _tfs.GetService<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tfs.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

