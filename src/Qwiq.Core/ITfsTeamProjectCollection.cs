using Microsoft.Qwiq.Credentials;
using System;

namespace Microsoft.Qwiq
{
    public interface ITfsTeamProjectCollection
    {
        /// <summary>Gets the credentials for this project collection.</summary>
        TfsCredentials AuthorizedCredentials { get; }

        /// <summary>
        ///     The identity who the calls to the server are being made for.
        /// </summary>
        ITeamFoundationIdentity AuthorizedIdentity { get; }

        ICommonStructureService CommonStructureService { get; }

        /// <summary> Returns true if this object has successfully authenticated. </summary>
        bool HasAuthenticated { get; }

        IIdentityManagementService IdentityManagementService { get; }

        /// <summary> This is used to convert dates and times to UTC. </summary>
        TimeZone TimeZone { get; }

        /// <summary>The base url for this connection</summary>
        Uri Uri { get; }
    }
}