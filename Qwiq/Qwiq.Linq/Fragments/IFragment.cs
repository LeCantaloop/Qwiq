using System;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    interface IFragment
    {
        string Get(Type queryType);

        bool IsValid();
    }
}
