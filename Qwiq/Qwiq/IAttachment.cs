using System;

namespace Microsoft.IE.Qwiq
{
    public interface IAttachment
    {
        DateTime AttachedTime { get; }
        string Comment { get; set; }
        DateTime CreationTime { get; }
        string Extension { get; }
        bool IsSaved { get; }
        DateTime LastWriteTime { get; }
        long Length { get; }
        string Name { get; }
        Uri Uri { get; }
    }
}