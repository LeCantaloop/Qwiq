using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class TfsTeamProjectCollectionProxy : ITfsTeamProjectCollection
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
    }
}
