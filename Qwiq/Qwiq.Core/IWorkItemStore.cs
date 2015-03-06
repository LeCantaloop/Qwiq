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
        IEnumerable<IWorkItem> Query(string wiql);
        IEnumerable<IWorkItem> Query(string wiql, IDictionary context, bool dayPrecision);
        IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql);
        IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, IDictionary context, bool dayPrecision);
        IEnumerable<IWorkItem> Query(IEnumerable<int> ids);
        IWorkItem Query(int id);
        ITfsTeamProjectCollection TeamProjectCollection { get; }
        IEnumerable<IProject> Projects { get; }
        IEnumerable<IWorkItemLinkType> WorkItemLinkTypes { get; }
    }
}
