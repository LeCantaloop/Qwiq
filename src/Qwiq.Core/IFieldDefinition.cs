using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public interface IFieldDefinition : IIdentifiable<int>, INamed
    {
        [NotNull]
        string ReferenceName { get; }
    }
}
