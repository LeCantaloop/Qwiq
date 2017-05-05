using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinition : IIdentifiable<int>
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        string ReferenceName { get; }
    }
}
