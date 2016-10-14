using System;

namespace Microsoft.Qwiq.Linq.Fragments
{
    internal class StringFragment : IFragment
    {
        private string Fragment { get; set; }

        public StringFragment(string fragment)
        {
            Fragment = fragment;
        }

        public virtual string Get(Type queryType)
        {
            return Fragment;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}

