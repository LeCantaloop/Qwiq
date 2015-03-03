using Tfs = Microsoft.TeamFoundation.Client;

namespace Microsoft.IE.Qwiq
{
    public class TfsTeamProjectCollection : ITfsTeamProjectCollection
    {
        private readonly Tfs.TfsTeamProjectCollection _tfs;

        public TfsTeamProjectCollection(Tfs.TfsTeamProjectCollection tfs)
        {
            _tfs = tfs;
        }

        // To do: we should not assume T will be Microsoft.IE.Qwiq.IIdentityManagementService2 all the time
        public IIdentityManagementService2 GetService<T>()
        {
            return new IdentityManagementService2(_tfs.GetService<TeamFoundation.Framework.Client.IIdentityManagementService2>());
        }
    }
}
