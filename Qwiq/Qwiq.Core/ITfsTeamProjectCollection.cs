using System;

namespace Microsoft.IE.Qwiq
{
    public interface ITfsTeamProjectCollection
    {
        IIdentityManagementService IdentityManagementService { get; }
        ICommonStructureService CommonStructureService { get; }
    }
}
