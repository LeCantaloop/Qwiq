using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// An exception that is thrown when a field definition does not exist on a work item type.
    /// </summary>
    /// <seealso cref="InvalidOperationException" />
    [Serializable]
    public class FieldDefinitionNotExistException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinitionNotExistException"/> class.
        /// </summary>
        public FieldDefinitionNotExistException()
            : base(
                   "TF26028: A field definition in the work item type definition file does not exist. Add a definition for this field, or remove the reference to the field and try again.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinitionNotExistException"/> class.
        /// </summary>
        /// <param name="name">The field definition name.</param>
        public FieldDefinitionNotExistException(string name)
            : base(
                   $"TF26027: A field definition {name} in the work item type definition file does not exist. Add a definition for this field or remove the reference to the field and try again."
                           .ToString(CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinitionNotExistException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
        public FieldDefinitionNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinitionNotExistException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected FieldDefinitionNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}