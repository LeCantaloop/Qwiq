namespace Microsoft.Qwiq.Credentials
{
    public interface IAuthenticationSuccessNotification : IAuthenticationNotification
    {
        ITfsTeamProjectCollection TeamProjectCollection { get; }
    }
}