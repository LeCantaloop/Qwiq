using JetBrains.Annotations;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IRevision : IWorkItemCore
    {
        /// <summary>
        /// Gets the attachments of the work item in this revision.
        /// </summary>
        IEnumerable<IAttachment> Attachments { get; }

        /// <summary>
        /// Gets the fields of the work item in this revision.
        /// </summary>
        [NotNull]
        IFieldCollection Fields { get; }

        /// <summary>
        /// Gets the index of this revision.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the links of the work item in this revision.
        /// </summary>
        IEnumerable<ILink> Links { get; }

        /// <summary>
        /// Gets the work item that is stored in this revision.
        /// </summary>
        [NotNull]
        IWorkItem WorkItem { get; }

        /// <summary>
        /// Gets the value of the specified field in the work item of this revision.
        /// </summary>
        /// <param name="name">The field of interest in the work item of this revision.</param>
        /// <returns>The value of the specified field.</returns>
        [CanBeNull]
        new object this[[NotNull] string name] { get; }

        /// <summary>
        /// Gets the tagline for this revision.
        /// </summary>
        /// <returns>Returns System.String.</returns>
        string GetTagLine();
    }
}