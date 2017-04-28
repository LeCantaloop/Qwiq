using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    [ContractClass(typeof(RevisionInternalContract))]
    internal interface IRevisionInternal
    {
        [JetBrains.Annotations.Pure]
        [CanBeNull]
        object GetCurrentFieldValue([NotNull] IFieldDefinition fieldDefinition);

        void SetFieldValue([NotNull] IFieldDefinition fieldDefinition, [CanBeNull] object value);
    }
}