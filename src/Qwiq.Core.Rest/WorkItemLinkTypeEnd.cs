using System;
using System.Diagnostics.Contracts;


using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class WorkItemLinkTypeEnd : Qwiq.WorkItemLinkTypeEnd, IIdentifiable<int?>
    {
        internal WorkItemLinkTypeEnd(WorkItemRelationType item)
            : base(item.ReferenceName)
        {
            Contract.Requires(item != null);

            ArgumentNullException.ThrowIfNull(item);
            Name = string.Intern(item.Name);
        }

        /// <inheritdoc />
        public int? Id { get; internal set; }
    }
}