namespace Microsoft.Qwiq
{
    public interface IReadOnlyField : IIdentifiable<int>
    {
        string Name { get; }
        string ReferenceName { get; }
        object Value { get; }
    }
}