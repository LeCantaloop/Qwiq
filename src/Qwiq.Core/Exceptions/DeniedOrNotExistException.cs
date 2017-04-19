using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq
{
    [Serializable]
    public class DeniedOrNotExistException : Exception
    {
        private const string ErrorFormat = "TF26193: The team project {0} does not exist. Check the team project name and try again.";

        public DeniedOrNotExistException()
            : base("TF237090: Does not exist or access is denied.")
        {
        }

        public DeniedOrNotExistException(string projectName)
            : base(string.Format(CultureInfo.InvariantCulture, ErrorFormat, projectName))
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public DeniedOrNotExistException(Guid projectGuid)
            : base(string.Format(CultureInfo.InvariantCulture, ErrorFormat, projectGuid))
        {
        }

        protected DeniedOrNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}