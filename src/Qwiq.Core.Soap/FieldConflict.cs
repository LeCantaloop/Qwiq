using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    public class FieldConflict : IFieldConflict
    {
        private readonly Tfs.FieldConflict _field;

        public FieldConflict(Tfs.FieldConflict field)
        {
            _field = field;
        }

        public object BaselineValue => _field.BaselineValue;

        public string FieldReferenceName => _field.FieldReferenceName;

        public object LocalValue => _field.LocalValue;

        public object ServerValue => _field.ServerValue;
    }
}
