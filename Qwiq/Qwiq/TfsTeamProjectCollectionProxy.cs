using Tfs = Microsoft.TeamFoundation.Client;

namespace Microsoft.IE.Qwiq
{
    public class TfsTeamProjectCollectionProxy : ITfsTeamProjectCollection
    {
        private readonly Tfs.TfsTeamProjectCollection _tfs;

        public TfsTeamProjectCollectionProxy(Tfs.TfsTeamProjectCollection tfs)
        {
            _tfs = tfs;
        }

        // To do: we should not assume T will be Microsoft.IE.Qwiq.IIdentityManagementService2 all the time
        public IIdentityManagementService2 GetService<T>()
        {
            return new IdentityManagementService2Proxy(_tfs.GetService<TeamFoundation.Framework.Client.IIdentityManagementService2>());
        }
    }
}
