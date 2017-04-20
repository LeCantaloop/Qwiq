using System;

namespace Microsoft.Qwiq.Credentials
{
    [Flags]
    public enum AuthenticationTypes
    {
        None,
        OpenAuthorization,
        PersonalAccessToken,
        Basic,
        Windows,
        All = None | OpenAuthorization | PersonalAccessToken | Basic | Windows
    }
}