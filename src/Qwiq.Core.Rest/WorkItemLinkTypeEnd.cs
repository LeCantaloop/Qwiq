using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class WorkItemLinkTypeEnd : Qwiq.WorkItemLinkTypeEnd, IIdentifiable<int?>
    {
        internal WorkItemLinkTypeEnd([NotNull] WorkItemRelationType item)
            : base(item.ReferenceName)
        {
            Contract.Requires(item != null);

            if (item == null) throw new ArgumentNullException(nameof(item));
            Name = string.Intern(item.Name);
        }

        /// <inheritdoc />
        public int? Id { get; internal set; }
    }
}