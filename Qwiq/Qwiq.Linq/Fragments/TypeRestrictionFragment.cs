using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class TypeRestrictionFragment : IFragment
    {
        private readonly ICollection<string> _workItemTypes;

        public TypeRestrictionFragment(ICollection<string> workItemTypes)
        {
            _workItemTypes = workItemTypes;
        }

        public string Get(Type queryType)
        {
            var numberOfTypesGreaterThanOne = _workItemTypes.Count() > 1;

            var format = numberOfTypesGreaterThanOne ? "([Work Item Type] IN ({0}))" : "([Work Item Type] = {0})";
            var replacement = string.Join(", ", _workItemTypes.Select(t => "'" + t + "'"));
            return string.Format(format, replacement);
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
