using System;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class TypeRestrictionFragment : IFragment
    {
        private readonly string _workItemType;

        public TypeRestrictionFragment(string workItemType)
        {
            _workItemType = workItemType;
        }

        public string Get(Type queryType)
        {
            return string.Format("([Work Item Type] = '{0}')", _workItemType);
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
