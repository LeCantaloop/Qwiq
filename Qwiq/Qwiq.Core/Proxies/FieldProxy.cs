using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class FieldProxy : IField
    {
        private readonly Tfs.Field _field;

        internal FieldProxy(Tfs.Field field)
        {
            _field = field;
        }

        public int Id
        {
            get { return _field.Id; }
        }

        public bool IsDirty
        {
            get { return _field.IsDirty; }
        }

        public bool IsEditable
        {
            get { return _field.IsEditable; }
        }

        public bool IsRequired
        {
            get { return _field.IsRequired; }
        }

        public bool IsValid
        {
            get { return _field.IsValid; }
        }

        public string Name
        {
            get { return _field.Name; }
        }

        public ValidationState ValidationState
        {
            get
            {
                return (ValidationState)((int)_field.Status);
            }
        }

        public bool IsChangedByUser
        {
            get { return _field.IsChangedByUser; }
        }

        public object Value
        {
            get { return _field.Value; }
            set { _field.Value = value; }
        }

        public object OriginalValue
        {
            get { return _field.OriginalValue; }
            set { _field.Value = value; }
        }
    }
}