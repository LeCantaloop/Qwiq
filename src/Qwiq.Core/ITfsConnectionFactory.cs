using Qwiq.Credentials;

namespace Qwiq
{
    public interface ITfsConnectionFactory
    {
        ITeamProjectCollection Create(AuthenticationOptions options);
    }
}