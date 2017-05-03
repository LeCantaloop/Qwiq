using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// Represents a failure to load a team foundation resource.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class DeniedOrNotExistException : Exception
    {
        private const string ErrorFormat = "TF26193: The team project {0} does not exist. Check the team project name and try again.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeniedOrNotExistException"/> class.
        /// </summary>
        public DeniedOrNotExistException()
            : base("TF237090: Does not exist or access is denied.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeniedOrNotExistException"/> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public DeniedOrNotExistException(string projectName)
            : base(string.Format(CultureInfo.InvariantCulture, ErrorFormat, projectName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeniedOrNotExistException"/> class.
        /// </summary>
        /// <param name="projectGuid">The project unique identifier.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public DeniedOrNotExistException(Guid projectGuid)
            : base(string.Format(CultureInfo.InvariantCulture, ErrorFormat, projectGuid))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeniedOrNotExistException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal DeniedOrNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeniedOrNotExistException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected DeniedOrNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}