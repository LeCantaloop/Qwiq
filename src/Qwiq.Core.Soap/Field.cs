using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Field : IField
    {
        private readonly Tfs.Field _field;

        internal Field(Tfs.Field field)
        {
            _field = field;
        }

        public int Id => _field.Id;

        public bool IsChangedByUser => _field.IsChangedByUser;

        public bool IsDirty => _field.IsDirty;

        public bool IsEditable => _field.IsEditable;

        public bool IsRequired => _field.IsRequired;

        public bool IsValid => _field.IsValid;

        public string Name => _field.Name;

        public string ReferenceName => _field.ReferenceName;

        public object OriginalValue
        {
            get => _field.OriginalValue;
            set => _field.Value = value;
        }

        public ValidationState ValidationState => (ValidationState)(int)_field.Status;

        public object Value
        {
            get => _field.Value;
            set => _field.Value = value;
        }
    }
}