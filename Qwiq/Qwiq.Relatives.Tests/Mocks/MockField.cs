namespace Microsoft.IE.Qwiq.Relatives.Tests.Mocks
{
    public class MockField : IField
    {
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsEditable { get; set; }
        public bool IsRequired { get; set; }
        public bool IsValid { get; set; }
        public string Name { get; set; }
        public object OriginalValue { get; set; }
        public ValidationState ValidationState { get; set; }
        public bool IsChangedByUser { get; }
        public object Value { get; set; }
    }
}
