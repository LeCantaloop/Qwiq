using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Linq
{
    /// <summary>
    /// Maps friendly names to and from reference names
    /// </summary>
    public interface IFieldMapper
    {
        /// <summary>
        /// Given a work item sub-type, get the TFS unambiguous [Work Item Type] value.
        /// </summary>
        /// <param name="type">The type of <see cref="IWorkItem"/> to get the name of.</param>
        /// <returns>The [Work Item Type] name for the sub-type.</returns>
        IEnumerable<string> GetWorkItemType(Type type);

        /// <summary>
        /// Given a specific work item sub-type, get the TFS field names needed to populate the type.
        /// </summary>
        /// <param name="type">The type of <see cref="IWorkItem"/> to get the fields for.</param>
        /// <returns>The list of strings of field names for the sub-type.</returns>
        IEnumerable<string> GetFieldNames(Type type);

        /// <summary>
        /// Given a specific work item sub-type and property name, get the associated TFS field name.
        /// </summary>
        /// <param name="type">The type of <see cref="IWorkItem"/> to get the field for.</param>
        /// <param name="propertyName">The property name on the sub-type to lookup</param>
        /// <example>
        /// For example, calling GetFieldName(typeof(Bug), "OpenedBy") returns "[Created By]"
        /// </example>
        /// <returns>The TFS field name that matches the property name.</returns>
        string GetFieldName(Type type, string propertyName);
    }
}