namespace Qwiq
{
    public interface ILink
    {
        BaseLinkType BaseType { get; }

        string Comment { get; }
    }
}