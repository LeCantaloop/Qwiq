using System;
using System.Diagnostics;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public abstract class Link : ILink
    {
        [DebuggerStepThrough]
        protected internal Link([CanBeNull] string comment, BaseLinkType baseType)
        {
            if (baseType == BaseLinkType.None) throw new ArgumentOutOfRangeException(nameof(baseType));

            Comment = comment;
            BaseType = baseType;
        }

        /// <inheritdoc />
        public virtual BaseLinkType BaseType { get; }

        /// <inheritdoc />
        public virtual string Comment { get; }
    }
}