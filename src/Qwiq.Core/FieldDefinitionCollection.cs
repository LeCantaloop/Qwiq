using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Proxies;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// A facade over <see cref="Microsoft.TeamFoundation.WorkItemTracking.Client.FieldDefinitionCollection"/>.
    /// </summary>
    public abstract class FieldDefinitionCollection : IFieldDefinitionCollection
    {
        public abstract int Count { get; }

        public abstract IFieldDefinition this[string name] { get; }

        public abstract bool Contains(string fieldName);

        public abstract IEnumerator<IFieldDefinition> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var fdc = obj as IFieldDefinitionCollection;
            if (fdc == null) return false;

            return this.All(p => fdc.Contains(p, FieldDefinitionComparer.Instance));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.Aggregate(27, (current, f) => (13 * current) ^ f.GetHashCode());
            }
        }
    }
}