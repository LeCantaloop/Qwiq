using System;

namespace Microsoft.Qwiq
{
    public interface IAttachment : IResourceReference
    {
        DateTime AttachedTime { get; }
        string Comment { get; set; }
        DateTime CreationTime { get; }
        string Extension { get; }
        bool IsSaved { get; }
        DateTime LastWriteTime { get; }
        long Length { get; }
        string Name { get; }
    }
}
