using System;

using Microsoft.VisualStudio.Services.WebApi;

namespace Qwiq.Client.Soap
{
    internal interface IInternalTeamProjectCollection : ITeamProjectCollection, IDisposable
    {
        T GetClient<T>()
            where T : VssHttpClientBase;

        T GetService<T>();
    }
}