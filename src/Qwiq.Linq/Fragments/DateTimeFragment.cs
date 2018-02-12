using System;

namespace Qwiq.Linq.Fragments
{
    internal class DateTimeFragment : ConstantFragment
    {
        public DateTimeFragment(DateTime constant)
            : base(constant.ToUniversalTime().ToString("u"))
        {
        }
    }
}

