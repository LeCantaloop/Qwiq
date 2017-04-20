using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Qwiq.Rest
{
    internal interface IInternalTeamProjectCollection : ITeamProjectCollection
    {
        T GetClient<T>()
            where T : VssHttpClientBase;

        T GetService<T>()
            where T : IVssClientService;
    }
}