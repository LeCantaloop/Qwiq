using System.Diagnostics.Contracts;

namespace Qwiq
{
    [ContractClassFor(typeof(IRevisionInternal))]
    internal abstract class RevisionInternalContract : IRevisionInternal
    {
        public object GetCurrentFieldValue(IFieldDefinition fieldDefinition)
        {
            Contract.Requires(fieldDefinition != null);

            return default(object);
        }

        public void SetFieldValue(IFieldDefinition fieldDefinition, object value)
        {
            Contract.Requires(fieldDefinition != null);
        }
    }
}