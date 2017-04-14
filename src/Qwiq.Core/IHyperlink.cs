using System;

namespace Microsoft.Qwiq
{
    public interface IHyperlink : ILink, IEquatable<IHyperlink>
    {
        string Location { get; }
    }
}