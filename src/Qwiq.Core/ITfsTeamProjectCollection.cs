using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.Services.Common;

namespace Qwiq
{
    /// <summary>
    /// Represents a connection to a Team Foundation Server or Azure DevOps organization.
    /// </summary>
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "ITeamProjectCollection matches the TFS/Azure DevOps API naming convention (TfsTeamProjectCollection).")]
    public interface ITeamProjectCollection : IResourceReference
    {
        /// <summary>Gets the credentials for this project collection.</summary>
        VssCredentials AuthorizedCredentials { get; }

        /// <summary>
        ///     The identity who the calls to the server are being made for.
        /// </summary>
        ITeamFoundationIdentity AuthorizedIdentity { get; }

        ICommonStructureService? CommonStructureService { get; }

        /// <summary> Returns true if this object has successfully authenticated. </summary>
        bool HasAuthenticated { get; }

        /// <summary> This is used to convert dates and times to UTC. </summary>
        TimeZone TimeZone { get; }
    }
}