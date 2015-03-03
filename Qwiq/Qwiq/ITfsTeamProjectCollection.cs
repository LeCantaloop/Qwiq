namespace Microsoft.IE.Qwiq
{
    public interface ITfsTeamProjectCollection
    {
        IIdentityManagementService2 GetService<T>();
    }
}
