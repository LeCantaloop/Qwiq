using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class FieldConflictProxy : IFieldConflict
    {
        private readonly Tfs.FieldConflict _field;

        public FieldConflictProxy(Tfs.FieldConflict field)
        {
            _field = field;
        }

        public object BaselineValue
        {
            get { return _field.BaselineValue; }
        }

        public string FieldReferenceName
        {
            get { return _field.FieldReferenceName; }
        }

        public object LocalValue
        {
            get { return _field.LocalValue; }
        }

        public object ServerValue
        {
            get { return _field.ServerValue; }
        }
    }
}
