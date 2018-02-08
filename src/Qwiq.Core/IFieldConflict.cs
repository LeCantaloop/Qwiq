namespace Qwiq
{
    public interface IFieldConflict
    {
        object BaselineValue { get; }
        string FieldReferenceName { get; }
        object LocalValue { get; }
        object ServerValue { get; }
    }
}
