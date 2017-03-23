using System;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public interface ITfsTeamProjectCollection
    {
        TfsCredentials AuthorizedCredentials { get; }

        ITeamFoundationIdentity AuthorizedIdentity { get; }

        ICommonStructureService CommonStructureService { get; }

        bool HasAuthenticated { get; }

        IIdentityManagementService IdentityManagementService { get; }
    }

    internal interface IInternalTfsTeamProjectCollection : ITfsTeamProjectCollection, IDisposable
    {
        T GetService<T>();

        T GetClient<T>();
    }
}