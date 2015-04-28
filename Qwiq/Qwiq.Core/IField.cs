namespace Microsoft.IE.Qwiq
{
    public interface IField
    {
        int Id { get; }
        bool IsRequired { get; }
        bool IsValid { get; }
        string Name { get; }
        object Value { get; set; }
        object OriginalValue { get; set; }
    }
}