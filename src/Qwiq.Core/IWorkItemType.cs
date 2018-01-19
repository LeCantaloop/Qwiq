namespace Microsoft.Qwiq
{
    public interface IWorkItemType : INamed
    {
        string Description { get; }
        IFieldDefinitionCollection FieldDefinitions { get; }
        IWorkItem NewWorkItem();
    }
}
