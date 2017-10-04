using System;

namespace Qwiq
{
    public interface IQueryDefinition : IIdentifiable<Guid>, IEquatable<IQueryDefinition>
    {
        string Name { get; }
        string Wiql { get; }
    }
}