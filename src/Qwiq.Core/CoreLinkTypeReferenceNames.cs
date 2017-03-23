using System.Collections.Generic;

namespace Microsoft.Qwiq
{
  public static class CoreLinkTypeReferenceNames
  {
    public static readonly string Related = "System.LinkTypes.Related";
    public static readonly string Hierarchy = "System.LinkTypes.Hierarchy";
    public static readonly string Dependency = "System.LinkTypes.Dependency";
    public static readonly string Duplicate = "System.LinkTypes.Duplicate";
    /// <summary>
    /// private static array that keeps all reference names of corefield
    /// so that caller can do enumeration easily
    /// </summary>
    private static readonly string[] _all = {
                                        Related,
                                        Hierarchy,
                                        Dependency,
                                        Duplicate
                                      };

    /// <summary>Returns the set of all core link types.</summary>
    public static IEnumerable<string> All => _all;
  }
}
