using System;

namespace Qwiq.Linq.Fragments
{
    interface IFragment
    {
        string Get(Type queryType);

        bool IsValid();
    }
}

