using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public interface ITfsConnectionFactory
    {
        ITeamProjectCollection Create(AuthenticationOptions options);
    }
}