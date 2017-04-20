using System;

namespace Microsoft.Qwiq.Soap
{
    internal interface IInternalTeamProjectCollection : ITeamProjectCollection, IDisposable
    {
        T GetClient<T>();

        T GetService<T>();
    }
}