namespace Microsoft.Qwiq
{
    public interface IField
    {
        int Id { get; }
        bool IsDirty { get; }
        bool IsEditable { get; }
        bool IsRequired { get; }
        bool IsValid { get; }
        string Name { get; }
        string ReferenceName { get; }
        object OriginalValue { get; set; }
        ValidationState ValidationState { get; }
        bool IsChangedByUser { get; }
        object Value { get; set; }
    }
}
