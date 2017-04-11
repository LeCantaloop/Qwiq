namespace Microsoft.Qwiq
{
    public interface IFieldDefinition : IIdentifiable<int>
    {
        string Name { get; }
        string ReferenceName { get; }
    }
}
