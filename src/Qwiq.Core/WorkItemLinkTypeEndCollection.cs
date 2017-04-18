using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeEndCollection : ReadOnlyListWithId<IWorkItemLinkTypeEnd, int>,
                                                 IWorkItemLinkTypeEndCollection
    {
        internal WorkItemLinkTypeEndCollection(IEnumerable<IWorkItemLinkType> linkTypes)
            : this(
                linkTypes.SelectMany(s => new[] { s.ForwardEnd, s.IsDirectional ? s.ReverseEnd : null })
                         .Where(p => p != null))
        {
        }

        internal WorkItemLinkTypeEndCollection(IEnumerable<IWorkItemLinkTypeEnd> linkEndTypes)
            : base(linkEndTypes, e => e.Name, e => e.Id)
        {
        }

        protected override void Add(IWorkItemLinkTypeEnd value, int index)
        {
            base.Add(value, index);
            AddByName(value.ImmutableName, index);
        }
    }
}