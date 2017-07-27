using System;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq
{
    public interface ITeamProjectCollection
    {
        /// <summary>Gets the credentials for this project collection.</summary>
        VssCredentials AuthorizedCredentials { get; }

        /// <summary>
        ///     The identity who the calls to the server are being made for.
        /// </summary>
        ITeamFoundationIdentity AuthorizedIdentity { get; }

        ICommonStructureService CommonStructureService { get; }

        /// <summary> Returns true if this object has successfully authenticated. </summary>
        bool HasAuthenticated { get; }

        /// <summary> This is used to convert dates and times to UTC. </summary>
        TimeZone TimeZone { get; }

        /// <summary>The base url for this connection</summary>
        Uri Uri { get; }
    }
}