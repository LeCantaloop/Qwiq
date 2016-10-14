using System;

namespace Microsoft.Qwiq.Linq.Fragments
{
    interface IFragment
    {
        string Get(Type queryType);

        bool IsValid();
    }
}

