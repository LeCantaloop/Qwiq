using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal class FieldDefinition : Qwiq.FieldDefinition
    {
        internal FieldDefinition(Tfs.FieldDefinition fieldDefinition)
            :base(fieldDefinition?.Id ?? 0, fieldDefinition?.ReferenceName, fieldDefinition?.Name)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Tfs.FieldDefinition"/> to <see cref="FieldDefinition"/>.
        /// </summary>
        /// <param name="fieldDefinition">The field definition.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator FieldDefinition(Tfs.FieldDefinition fieldDefinition)
        {
            return new FieldDefinition(fieldDefinition);
        }
    }
}