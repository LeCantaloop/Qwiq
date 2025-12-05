using System.Diagnostics.Contracts;


namespace Qwiq
{
    [ContractClass(typeof(RevisionInternalContract))]
    internal interface IRevisionInternal
    {
        object? GetCurrentFieldValue(IFieldDefinition fieldDefinition);

        void SetFieldValue(IFieldDefinition fieldDefinition, object? value);
    }
}