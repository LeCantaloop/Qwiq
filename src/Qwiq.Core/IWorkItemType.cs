namespace Microsoft.Qwiq
{
    public interface IWorkItemType
    {
        string Description { get; }
        string Name { get; }

        IFieldDefinitionCollection FieldDefinitions { get; }
        IWorkItem NewWorkItem();
    }
}
