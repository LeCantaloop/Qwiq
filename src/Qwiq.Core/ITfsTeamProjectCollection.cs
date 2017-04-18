using System;

namespace Microsoft.Qwiq
{
    public interface ITfsTeamProjectCollection
    {

        ICommonStructureService CommonStructureService { get; }

        IIdentityManagementService IdentityManagementService { get; }

        /// <summary> This is used to convert dates and times to UTC. </summary>
        TimeZone TimeZone { get; }

        /// <summary>The base url for this connection</summary>
        Uri Uri { get; }
    }
}