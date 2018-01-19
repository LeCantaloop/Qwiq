using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Microsoft.Qwiq
{

    public class WorkItemClassificationNode<TId> : IWorkItemClassificationNode<TId>, IEquatable<IWorkItemClassificationNode<TId>>
    {
        

        public WorkItemClassificationNode(TId id, NodeType nodeType, [NotNull] string name, Uri uri)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));


            Name = name;
            Uri = uri;
            Id = id;
            Type = nodeType;
        }

        public TId Id { get; }
        public Uri Uri { get; }
        public NodeType Type { get; }
        public string Name { get; }

        [DebuggerStepThrough]
        public bool Equals(IWorkItemClassificationNode<TId> other)
        {
            return WorkItemClassificationNodeComparer<TId>.Default.Equals(this, other);
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return WorkItemClassificationNodeComparer<TId>.Default.Equals(this, obj as IWorkItemClassificationNode<TId>);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return WorkItemClassificationNodeComparer<TId>.Default.GetHashCode(this);
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return Name;
        }
    }
}
