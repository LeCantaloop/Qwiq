using System;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq
{
    [Serializable]
    public class DeniedOrNotExistException : ApplicationException
    {
        private static readonly string ErrorFormat =
            "TF26193: The team project {0} does not exist. Check the team project name and try again.";

        public DeniedOrNotExistException()
            : base("TF237090: Does not exist or access is denied.")
        {
        }

        public DeniedOrNotExistException(string projectName)
            : base(string.Format(ErrorFormat, projectName))
        {
        }

        public DeniedOrNotExistException(Guid projectGuid)
            : base(string.Format(ErrorFormat, projectGuid))
        {
        }

        protected DeniedOrNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}