using System;
using System.Collections.Generic;

namespace Qwiq
{
    public class QueryDefinitionCollection : ReadOnlyObjectWithIdCollection<IQueryDefinition, Guid>, IQueryDefinitionCollection
    {
        internal QueryDefinitionCollection(Func<IEnumerable<IQueryDefinition>> queryDefinitionFactory)
            : base(queryDefinitionFactory, queryDefinition => queryDefinition.Name)
        {
        }

        public override bool Equals(object obj)
        {
            return Comparer.QueryDefinitionCollection.Equals(this, obj as IQueryDefinitionCollection);
        }

        public bool Equals(IQueryDefinitionCollection other)
        {
            return Comparer.QueryDefinitionCollection.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return Comparer.QueryDefinitionCollection.GetHashCode(this);
        }
    }
}