namespace Microsoft.Qwiq.Proxies
{
    internal partial class FieldDefinitionProxy : IFieldDefinition
    {
        public string Name { get; }

        public string ReferenceName { get; }

        public override bool Equals(object obj)
        {
            return FieldDefinitionComparer.Instance.Equals(this, obj as IFieldDefinition);
        }

        public override int GetHashCode()
        {
            return FieldDefinitionComparer.Instance.GetHashCode(this);
        }
    }
}