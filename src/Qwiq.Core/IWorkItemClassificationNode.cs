namespace Qwiq
{
    public interface IWorkItemClassificationNode<out TId> : IIdentifiable<TId>, IResourceReference, INamed
    {
        NodeType Type { get; }
    }
}
