using System;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// Legacy interface
    /// </summary>
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

    public interface IReadOnlyField : IIdentifiable<int>
    {
        string Name { get; }
        string ReferenceName { get; }
        object Value { get; }
    }
}
