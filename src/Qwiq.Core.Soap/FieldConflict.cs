using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class FieldConflict : IFieldConflict
    {
        private readonly Tfs.FieldConflict _field;

        internal FieldConflict(Tfs.FieldConflict field)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public object BaselineValue => _field.BaselineValue;

        public string FieldReferenceName => _field.FieldReferenceName;

        public object LocalValue => _field.LocalValue;

        public object ServerValue => _field.ServerValue;
    }
}
