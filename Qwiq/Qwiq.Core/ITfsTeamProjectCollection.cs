using System;

namespace Microsoft.Qwiq
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

