using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public interface IAuthenticationOptions
    {
        Uri Uri { get; }

        AuthenticationType AuthenticationType { get; }

        IEnumerable<VssCredentials> Credentials { get; }

        ICredentialsNotifications Notifications { get; }
    }
}
