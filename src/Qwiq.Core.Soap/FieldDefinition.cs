using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal class FieldDefinition : Qwiq.FieldDefinition
    {
        internal FieldDefinition(Tfs.FieldDefinition fieldDefinition)
            : base(
                (fieldDefinition ?? throw new ArgumentNullException(nameof(fieldDefinition))).Id,
                fieldDefinition.ReferenceName,
                fieldDefinition.Name)
        {
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