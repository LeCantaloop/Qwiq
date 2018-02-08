using JetBrains.Annotations;

namespace Qwiq
{
    public interface IFieldDefinition : IIdentifiable<int>, INamed
    {
        [NotNull]
        string ReferenceName { get; }
    }
}
