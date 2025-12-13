using System;

namespace Qwiq
{
    public interface IQueryDefinitionCollection : IReadOnlyObjectWithIdCollection<IQueryDefinition, Guid>, IEquatable<IQueryDefinitionCollection>
    {
    }
}