using System;
using System.Diagnostics.CodeAnalysis;

namespace Qwiq
{
    /// <summary>
    /// Specifies options for saving work items.
    /// </summary>
    [Flags]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "SaveFlags matches the TFS/Azure DevOps API naming convention for flags enums.")]
    public enum SaveFlags
    {
        None = 0,
        MergeLinks = 1,
        MergeAll = 2,
    }
}
