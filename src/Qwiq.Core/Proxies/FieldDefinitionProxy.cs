namespace Microsoft.Qwiq.Proxies
{
    internal class FieldDefinitionProxy : IFieldDefinition
    {
        public string Name { get; internal set; }

        public string ReferenceName { get; internal set; }

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