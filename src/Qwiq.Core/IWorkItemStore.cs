using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public interface IWorkItemStore : IDisposable
    {
        IEnumerable<IProject> Projects { get; }

        ITfsTeamProjectCollection TeamProjectCollection { get; }

        TimeZone TimeZone { get; }

        string UserDisplayName { get; }

        string UserIdentityName { get; }

        string UserSid { get; }

        IEnumerable<IWorkItemLinkType> WorkItemLinkTypes { get; }

        IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false);

        IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null);

        IWorkItem Query(int id, DateTime? asOf = null);

        IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false);
    }
}