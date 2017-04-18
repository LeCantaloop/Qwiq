using System;
using System.Threading.Tasks;

namespace Microsoft.Qwiq.Credentials
{
    public interface ICredentialsNotifications
    {
        Func<IAuthenticationFailedNotification, Task> AuthenticationFailed { get; set; }
        Func<IAuthenticationSuccessNotification, Task> AuthenticationSuccess { get; set; }
    }
}