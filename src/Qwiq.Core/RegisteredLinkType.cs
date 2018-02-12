using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Qwiq
{
    public class RegisteredLinkType : IRegisteredLinkType
    {
        public RegisteredLinkType([NotNull] string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Name = name != null ? string.Intern(name) : throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}