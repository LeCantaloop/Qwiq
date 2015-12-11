using System;

namespace Microsoft.IE.Qwiq
{
    public interface ITfsTeamProjectCollection
    {
        IIdentityManagementService IdentityManagementService { get; }
        ICommonStructureService CommonStructureService { get; }
    }

    internal interface IInternalTfsTeamProjectCollection : ITfsTeamProjectCollection, IDisposable
    {
        T GetService<T>();
    }
}
