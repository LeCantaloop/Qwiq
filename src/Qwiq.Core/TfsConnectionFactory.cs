using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public interface ITfsConnectionFactory
    {
        ITeamProjectCollection Create(AuthenticationOptions options);
    }

    public abstract class TfsConnectionFactory : ITfsConnectionFactory
    {
        public abstract ITeamProjectCollection Create(AuthenticationOptions options);
    }
}
