
namespace Qwiq
{
    public interface IFieldDefinition : IIdentifiable<int>, INamed
    {
        string ReferenceName { get; }
    }
}
