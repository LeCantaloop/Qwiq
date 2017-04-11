namespace Microsoft.Qwiq
{
    /// <summary>
    ///     This allows an object to be identified, <typeparam name="TKey" /> is the identifier (AKA Key)
    /// </summary>
    /// <typeparam name="TKey">The identifier (AKA Key)</typeparam>
    public interface IIdentifiable<out TKey>
    {
        TKey Id { get; }
    }
}