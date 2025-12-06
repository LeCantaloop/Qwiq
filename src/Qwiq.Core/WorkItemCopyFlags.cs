using System;
using System.Diagnostics.CodeAnalysis;

namespace Qwiq
{
    /// <summary>
    /// Flags specifying optional work item data that should be copied.
    /// </summary>
    /// <remarks>
    /// This enum mirrors the values from the TFS Client OM's WorkItemCopyFlags.
    /// </remarks>
    [Flags]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "WorkItemCopyFlags matches the TFS/Azure DevOps API naming convention for flags enums.")]
    public enum WorkItemCopyFlags
    {
        None = 0,
        CopyFiles = 1,
        CopyLinks = 2,
    }
}
