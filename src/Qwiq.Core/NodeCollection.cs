using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class NodeCollection : ReadOnlyList, INodeCollection, IEquatable<INodeCollection>
    {
        private readonly Dictionary<string, int> _mapByName;

        private readonly IList<INode> _nodes;

        internal NodeCollection(params INode[] nodes)
            : this(nodes as IEnumerable<INode>)
        {
        }

        internal NodeCollection(IEnumerable<INode> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            _nodes = nodes.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();
            _mapByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < _nodes.Count; i++) _mapByName.Add(_nodes[i].Name, i);
        }

        public bool Equals(INodeCollection other)
        {
            return NodeCollectionComparer.Instance.Equals(this, other);
        }

        public override int Count => _nodes.Count;

        public INode this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Value cannot be null or empty.", nameof(name));

                if (_mapByName.TryGetValue(name, out int index)) return _nodes[index];

                throw new Exception();
            }
        }

        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            return _mapByName.ContainsKey(name);
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return NodeCollectionComparer.Instance.Equals(this, obj as INodeCollection);
        }

        public override int GetHashCode()
        {
            return NodeCollectionComparer.Instance.GetHashCode(this);
        }

        protected override object GetItem(int index)
        {
            return _nodes[index];
        }
    }
}