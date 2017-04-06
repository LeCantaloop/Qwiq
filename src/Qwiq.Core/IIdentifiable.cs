namespace Microsoft.Qwiq
{
    /// <summary>
    ///     This allows an object to be identified, K is the identifier (AKA Key)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IIdentifiable<out TKey> 
    {
        TKey Id { get; }
    }
}