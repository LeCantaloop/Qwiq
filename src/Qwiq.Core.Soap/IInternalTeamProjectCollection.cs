using System;

namespace Qwiq.Client.Soap
{
    internal interface IInternalTeamProjectCollection : ITeamProjectCollection, IDisposable
    {
        T GetClient<T>();

        T GetService<T>();
    }
}