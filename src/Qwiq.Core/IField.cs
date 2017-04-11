namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Legacy interface
    /// </summary>
    public interface IField : IReadOnlyField
    {
        bool IsChangedByUser { get; }

        bool IsDirty { get; }

        bool IsEditable { get; }

        bool IsRequired { get; }

        bool IsValid { get; }

        object OriginalValue { get; }

        ValidationState ValidationState { get; }

        new object Value { get; set; }
    }
}