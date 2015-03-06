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

        public object Value
        {
            get { return _field.Value; }
            set { _field.Value = value; }
        }
    }
}