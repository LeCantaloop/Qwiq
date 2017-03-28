using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal static class RegisteredLinkTypeMapper
    {
        public static Tfs.RegisteredLinkType Map(Tfs.WorkItemStore store, string linkTypeName)
            => store.RegisteredLinkTypes
                .OfType<Tfs.RegisteredLinkType>()
                .FirstOrDefault(x => x.Name == linkTypeName);
    }
}
