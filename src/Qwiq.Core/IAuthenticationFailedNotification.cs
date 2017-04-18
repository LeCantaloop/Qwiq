using System;

namespace Microsoft.Qwiq.Credentials
{
    public interface IAuthenticationFailedNotification : IAuthenticationNotification
    {
        Exception Exception { get; set; }
    }
}