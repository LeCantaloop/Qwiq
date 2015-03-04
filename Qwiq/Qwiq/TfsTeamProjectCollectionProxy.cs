using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.IE.Qwiq
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
                return new IdentityManagementServiceProxy(
                        _tfs.GetService<Tfs.Framework.Client.IIdentityManagementService2>());
            }
        }
    }
}
