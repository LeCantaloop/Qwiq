using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public interface IAuthenticationNotification
    {
        VssCredentials Credentials { get; }
    }
}