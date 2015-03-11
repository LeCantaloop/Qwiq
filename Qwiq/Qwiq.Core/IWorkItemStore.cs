using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    /// <summary>
    /// Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public interface IWorkItemStore : IDisposable
    {
        IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false);
        IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false);
        IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null);
        IWorkItem Query(int id, DateTime? asOf = null);
        ITfsTeamProjectCollection TeamProjectCollection { get; }
        IEnumerable<IProject> Projects { get; }
        IEnumerable<IWorkItemLinkType> WorkItemLinkTypes { get; }
    }
}
