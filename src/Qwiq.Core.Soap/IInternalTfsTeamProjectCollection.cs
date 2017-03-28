using System;

namespace Microsoft.Qwiq.Soap
{
    internal interface IInternalTfsTeamProjectCollection : ITfsTeamProjectCollection, IDisposable
    {
        T GetClient<T>();

        T GetService<T>();
    }
}