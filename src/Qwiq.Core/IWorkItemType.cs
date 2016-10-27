namespace Microsoft.Qwiq
{
    public interface IWorkItemType
    {
        string Description { get; }
        string Name { get; }
        IWorkItem NewWorkItem();
    }
}
