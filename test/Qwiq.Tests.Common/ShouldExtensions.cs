using System.Collections.Generic;
using System.Linq;

using Should;

namespace Microsoft.Qwiq.Tests.Common
{
    public static class ShouldExtensions
    {
        public static void ShouldContainOnly<T>(this IEnumerable<T> container, IEnumerable<T> contains)
        {
            container.All(contains.Contains).ShouldBeTrue();
        }
    }
}
