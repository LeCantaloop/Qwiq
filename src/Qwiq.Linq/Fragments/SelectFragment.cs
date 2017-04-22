using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Linq.Fragments
{
    internal class SelectFragment : IFragment
    {
        private readonly ICollection<string> _fields;

        public SelectFragment(ICollection<string> fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (fields.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(fields));
            _fields = fields;
        }

        public string Get(Type queryType)
        {
            return $"SELECT {string.Join(", ", _fields)} FROM {WiqlConstants.WorkItemTable}";
        }

        public bool IsValid()
        {
            return true;
        }
    }
}

