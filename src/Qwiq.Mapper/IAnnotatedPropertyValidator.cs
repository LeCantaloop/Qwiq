using JetBrains.Annotations;
using Qwiq.Mapper.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qwiq.Mapper
{
    /// <summary>
    /// Validates <see cref="FieldDefinitionAttribute"/> annotated properties
    /// </summary>
    public interface IAnnotatedPropertyValidator
    {
        /// <summary>
        /// Inspects a <see cref="PropertyInfo"/> and <see cref="IWorkItem"/> and returns true if the <see cref="PropertyInfo"/> is valid; otherwise, false.
        /// </summary>
        Func<IWorkItem, PropertyInfo, bool> PropertyInfoValidator { get; set; }

        /// <summary>
        /// Gets an instance of <see cref="FieldDefinitionAttribute"/> for a given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">An instance of <see cref="PropertyInfo"/> decorated with <see cref="FieldDefinitionAttribute"/>.</param>
        /// <returns>If the <paramref name="property"/> is decorated with <see cref="FieldDefinitionAttribute"/> then the attribute; otherwise, null.</returns>
        [CanBeNull]
        FieldDefinitionAttribute GetFieldDefinition([NotNull] PropertyInfo property);

        /// <summary>
        /// Gets and validates annotated properties of <paramref name="targetType"/> against <paramref name="workItem"/>.
        /// </summary>
        /// <param name="workItem">An instance of <see cref="IWorkItem"/>.</param>
        /// <param name="targetType">The type being mapped.</param>
        /// <returns>A collection of <see cref="PropertyInfo"/> dedocated with <see cref="FieldDefinitionAttribute"/> that are valid.</returns>
        [NotNull]
        IEnumerable<KeyValuePair<PropertyInfo, FieldDefinitionAttribute>> GetValidAnnotatedProperties([NotNull] IWorkItem workItem, [NotNull] Type targetType);
    }
}