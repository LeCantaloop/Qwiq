using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal static class RegisteredLinkTypeMapper
    {
        internal static Tfs.RegisteredLinkType Map(Tfs.WorkItemStore store, string linkTypeName)
            => store.RegisteredLinkTypes
                .OfType<Tfs.RegisteredLinkType>()
                .FirstOrDefault(x => x.Name == linkTypeName);
    }
}
