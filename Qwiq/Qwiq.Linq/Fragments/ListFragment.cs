using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class ListFragment : IFragment
    {
        private readonly IEnumerable<IFragment> _fragments;

        public ListFragment(IEnumerable<IFragment> fragments)
        {
            _fragments = fragments;
        }

        public string Get(Type queryType)
        {
            return "(" + String.Join(", ", _fragments.Select(s => s.Get(queryType))) + ")";
        }

        public bool IsValid()
        {
            return _fragments.Any() && _fragments.All(fragment => fragment.IsValid());
        }
    }
}