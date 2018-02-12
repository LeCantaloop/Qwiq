using System;
using System.Linq;

namespace Qwiq.Linq.Fragments
{
    internal class ConstantFragment : StringFragment
    {
        public ConstantFragment(string fragment)
            : base(fragment)
        {
        }

        public override string Get(Type queryType)
        {
            return "'" + EscapeSpecialWiqlChars(base.Get(queryType)) + "'";
        }

        private static string EscapeSpecialWiqlChars(string text)
        {
            // We make them strings instead of chars because .Replace(char, char) only allows a 1-for-1 substitution.
            // Because we're escaping we need to replace 1 char with 2.
            string[] specialChars = { "'" };
            const string escapeChar = "'";

            return specialChars.Aggregate(text, (current, specialChar) => current.Replace(specialChar, escapeChar + specialChar));
        }
    }
}

