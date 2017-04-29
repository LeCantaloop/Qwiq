using System;

namespace Microsoft.Qwiq.Client.Soap
{
    internal interface IInternalTeamProjectCollection : ITeamProjectCollection, IDisposable
    {
        T GetClient<T>();

        T GetService<T>();
    }
}