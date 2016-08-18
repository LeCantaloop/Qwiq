using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class NumberListFragment : ListFragment
    {
        public NumberListFragment(IEnumerable<int> numbers)
            : base(numbers.Select(number => new StringFragment(number.ToString())).ToArray())
        {
        }
    }
}
