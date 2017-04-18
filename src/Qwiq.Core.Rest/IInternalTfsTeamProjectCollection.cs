using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Qwiq.Rest
{
    internal interface IInternalTfsTeamProjectCollection : ITfsTeamProjectCollection
    {
        T GetClient<T>()
            where T : VssHttpClientBase;

        T GetService<T>()
            where T : IVssClientService;
    }
}