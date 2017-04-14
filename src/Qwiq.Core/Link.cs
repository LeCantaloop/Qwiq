using System.Diagnostics;

namespace Microsoft.Qwiq
{
    public abstract class Link : ILink
    {
        [DebuggerStepThrough]
        protected internal Link(string comment, BaseLinkType baseType)
        {
            Comment = comment;
            BaseType = baseType;
        }

        /// <inheritdoc />
        public virtual BaseLinkType BaseType { get; }

        /// <inheritdoc />
        public virtual string Comment { get; }
    }
}