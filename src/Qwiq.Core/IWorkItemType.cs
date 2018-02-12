namespace Qwiq
{
    public interface IWorkItemType : INamed
    {
        string Description { get; }
        IFieldDefinitionCollection FieldDefinitions { get; }
        IWorkItem NewWorkItem();
    }
}
