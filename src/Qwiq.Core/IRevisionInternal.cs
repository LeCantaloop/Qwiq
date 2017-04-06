namespace Microsoft.Qwiq
{
    internal interface IRevisionInternal
    {
        object GetCurrentFieldValue(IFieldDefinition fieldDefinition);

        void SetFieldValue(IFieldDefinition fieldDefinition, object value);
    }
}