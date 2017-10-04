using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Qwiq
{
    public class QueryDefinition : IQueryDefinition
    {
        public QueryDefinition(Guid id, [NotNull] string name, [NotNull] string wiql)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(wiql))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(wiql));
            }

            Id = id;
            Name = name;
            Wiql = wiql;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Wiql { get; }

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