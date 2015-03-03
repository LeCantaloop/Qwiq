using System;
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
        IEnumerable<IWorkItem> Query(IEnumerable<int> ids);
        IWorkItem Query(int id);
        IWorkItem Create(string type, string projectName);
        TeamFoundation.Client.TfsTeamProjectCollection TeamProjectCollection { get; }
    }
}
