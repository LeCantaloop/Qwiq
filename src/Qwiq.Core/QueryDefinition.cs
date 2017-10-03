using System;
using System.Globalization;

namespace Qwiq
{
    public class QueryDefinition : IQueryDefinition
    {
        public QueryDefinition(Guid id, string name, string queryText)
        {
            Id = id;
            Name = name;
            QueryText = queryText;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string QueryText { get; }

        public override bool Equals(object obj)
        {
            return QueryDefinitionComparer.Default.Equals(this, obj as IQueryDefinition);
        }

        public bool Equals(IQueryDefinition other)
        {
            return QueryDefinitionComparer.Default.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return QueryDefinitionComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            FormattableString s = $"{Id} ({Name})";
            return s.ToString(CultureInfo.InvariantCulture);
        }
    }
}