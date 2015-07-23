using System;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class WorkItemLinkAttribute : Attribute
    {
        private readonly Type _workItemType;
        private readonly string _linkImmutableName;

        public WorkItemLinkAttribute(Type workItemType, string linkImmutableName)
        {
            _workItemType = workItemType;
            _linkImmutableName = linkImmutableName;
        }

        public string GetLinkName()
        {
            return _linkImmutableName;
        }

        public Type GetWorkItemType()
        {
            return _workItemType;
        }
    }
}
