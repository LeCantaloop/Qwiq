using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Qwiq
{
    public abstract class QueryDefinition : IQueryDefinition
    {
        internal QueryDefinition(Guid id, [NotNull] string name, [NotNull] string wiql, [NotNull] string path)
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

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            }

            Id = id;
            Name = name;
            Wiql = wiql;
            Path = path;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Wiql { get; }
        public string Path { get; }

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