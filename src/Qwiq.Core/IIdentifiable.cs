namespace Qwiq
{
    /// <summary>
    ///     Allows an object to be identified by a key of type <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier (key).</typeparam>
    public interface IIdentifiable<out TKey>
    {
        TKey Id { get; }
    }
}