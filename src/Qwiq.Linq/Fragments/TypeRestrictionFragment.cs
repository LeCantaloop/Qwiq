using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Linq.Fragments
{
    internal class TypeRestrictionFragment : IFragment
    {
        private readonly HashSet<string> _workItemTypes;

        public TypeRestrictionFragment(IEnumerable<string> workItemTypes)
        {
            _workItemTypes = new HashSet<string>(workItemTypes, Comparer.OrdinalIgnoreCase);
        }

        public string Get(Type queryType)
        {
            var numberOfTypesGreaterThanOne = _workItemTypes.Count > 1;

            var format = numberOfTypesGreaterThanOne
                ? $"([{CoreFieldRefNames.WorkItemType}] IN ({{0}}))"
                : $"([{CoreFieldRefNames.WorkItemType}] = {{0}})";
            var replacement = string.Join(", ", _workItemTypes.Select(t => "'" + t + "'"));
            return string.Format(format, replacement);
        }

        public bool IsValid()
        {
            return true;
        }
    }
}

