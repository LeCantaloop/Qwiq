using System;
using System.Diagnostics;


namespace Qwiq
{
    public abstract class Link : ILink
    {
        [DebuggerStepThrough]
        protected internal Link(string comment, BaseLinkType baseType)
        {
            if (baseType == BaseLinkType.None) throw new ArgumentOutOfRangeException(nameof(baseType));

            Comment = comment;
            BaseType = baseType;
        }

        /// <inheritdoc />
        public BaseLinkType BaseType { get; }

        /// <inheritdoc />
        public string? Comment { get; }
    }
}