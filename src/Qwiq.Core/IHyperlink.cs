using System;

namespace Qwiq
{
    public interface IHyperlink : ILink, IEquatable<IHyperlink>
    {
        string Location { get; }
    }
}