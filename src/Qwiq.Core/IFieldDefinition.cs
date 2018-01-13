using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinition : IIdentifiable<int>, INamed
    {
        [NotNull]
        new string Name { get; }

        [NotNull]
        string ReferenceName { get; }
    }
}
