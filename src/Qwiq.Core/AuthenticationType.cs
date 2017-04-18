using System;

namespace Microsoft.Qwiq.Credentials
{
    [Flags]
    public enum AuthenticationType
    {
        Anonymous,
        OAuth,
        PersonalAccessToken,
        Basic,
        Windows,
        All = Anonymous | OAuth | PersonalAccessToken | Basic | Windows
    }
}