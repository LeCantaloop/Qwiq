using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwiq.Linq.Fragments
{
    internal class CompoundFragment : IFragment
    {
        private Queue<IFragment> Fragments { get; set; }

        public CompoundFragment(Queue<IFragment> fragments)
        {
            Fragments = fragments;
        }

        public string Get(Type queryType)
        {
            string retVal = string.Empty;
            while (Fragments.Count > 0)
            {
                var current = Fragments.Dequeue();
                retVal += current.Get(queryType);
            }

            return retVal;
        }

        public bool IsValid()
        {
            return Fragments.All(fragment => fragment.IsValid());
        }
    }
}

