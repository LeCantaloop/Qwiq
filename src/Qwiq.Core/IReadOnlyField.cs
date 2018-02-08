using JetBrains.Annotations;

namespace Qwiq
{
    public interface IReadOnlyField : IIdentifiable<int>
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        string ReferenceName { get; }

        [CanBeNull]
        object Value { get; }

        [NotNull]
        IFieldDefinition FieldDefinition { get; }
    }
}