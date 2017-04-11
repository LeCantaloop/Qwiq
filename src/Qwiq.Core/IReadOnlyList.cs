namespace Microsoft.Qwiq
{
    public interface IReadOnlyList<T> : System.Collections.Generic.IReadOnlyList<T>
    {
        T this[string name] { get; }

        bool Contains(T value);

        bool Contains(string name);

        T GetItem(int index);

        int IndexOf(T value);

        bool TryGetByName(string name, out T value);
    }
}