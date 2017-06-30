namespace Microsoft.Qwiq
{
    public interface IWorkItemReference : IIdentifiable<int?>
    {
        string Url { get; }
    }
}