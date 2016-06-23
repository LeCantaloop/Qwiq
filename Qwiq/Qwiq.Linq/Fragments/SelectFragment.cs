using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class SelectFragment : IFragment
    {
        private readonly ICollection<string> _fields;

        public SelectFragment(ICollection<string> fields)
        {
            _fields = fields;
        }

        public string Get(Type queryType)
        {
            return "SELECT " + string.Join(", ", _fields) + " FROM WorkItems";
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
