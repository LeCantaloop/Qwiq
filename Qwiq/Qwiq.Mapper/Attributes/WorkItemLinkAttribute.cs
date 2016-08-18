using System;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WorkItemLinkAttribute : Attribute
    {
        public WorkItemLinkAttribute(Type workItemType, string linkImmutableName)
        {
            WorkItemType = workItemType;
            LinkName = linkImmutableName;
        }

        public string LinkName { get; }

        public Type WorkItemType { get; }
    }
}
