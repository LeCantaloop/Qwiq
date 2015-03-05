using System;

namespace Microsoft.IE.Qwiq
{
    public interface ITfsTeamProjectCollection : IDisposable
    {
        IIdentityManagementService IdentityManagementService { get; }
        ICommonStructureService CommonStructureService { get; }
    }
}
