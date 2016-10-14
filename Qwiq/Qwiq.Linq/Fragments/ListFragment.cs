using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Linq.Fragments
{
    internal class ListFragment : IFragment
    {
        private readonly ICollection<IFragment> _fragments;

        public ListFragment(ICollection<IFragment> fragments)
        {
            _fragments = fragments;
        }

        public string Get(Type queryType)
        {
            return "(" + string.Join(", ", _fragments.Select(s => s.Get(queryType))) + ")";
        }

        public bool IsValid()
        {
            return _fragments.Any() && _fragments.All(fragment => fragment.IsValid());
        }
    }
}
