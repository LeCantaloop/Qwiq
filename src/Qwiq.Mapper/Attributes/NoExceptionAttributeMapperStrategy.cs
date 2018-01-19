using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public class NoExceptionAttributeMapperStrategy : AttributeMapperStrategy
    {
        /// <summary>
        /// Creates a default instance of <see cref="NoExceptionAttributeMapperStrategy"/> with <see cref="PropertyReflector"/>.
        /// </summary>
        public NoExceptionAttributeMapperStrategy()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="NoExceptionAttributeMapperStrategy"/> with the specified <paramref name="propertyReflector"/>.
        /// </summary>
        /// <param name="propertyReflector">An instance of <see cref="IPropertyReflector"/>.</param>
        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyReflector propertyReflector)
            : base(propertyReflector)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="NoExceptionAttributeMapperStrategy"/> with the specified <paramref name="inspector"/> and a default instance of <see cref="ITypeParser"/>.
        /// </summary>
        /// <param name="inspector">An instance of <see cref="IPropertyInspector"/>.</param>
        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyInspector inspector)
            : base(inspector)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="NoExceptionAttributeMapperStrategy"/> with a <see cref="AnnotatedPropertyValidator"/> using the specified <paramref name="inspector"/> and <paramref name="typeParser"/>.
        /// </summary>
        /// <param name="inspector">An instance of <see cref="IPropertyInspector"/>.</param>
        /// <param name="typeParser">An instance of <see cref="ITypeParser"/>.</param>
        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyInspector inspector, [NotNull] ITypeParser typeParser)
            : base(inspector, typeParser)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="NoExceptionAttributeMapperStrategy"/> with the specified <paramref name="annotatedPropertyValidator"/> and <paramref name="typeParser"/>.
        /// </summary>
        /// <param name="annotatedPropertyValidator">An instance of <see cref="IAnnotatedPropertyValidator"/>.</param>
        /// <param name="typeParser">An instance of <see cref="ITypeParser"/>.</param>
        public NoExceptionAttributeMapperStrategy([NotNull] IAnnotatedPropertyValidator annotatedPropertyValidator, [NotNull] ITypeParser typeParser)
            : base(annotatedPropertyValidator, typeParser)
        {

        }

        protected internal override void AssignFieldValue(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, PropertyInfo property, string fieldName, bool convert, object nullSub, object fieldValue)
        {
            try
            {
                base.AssignFieldValue(targetWorkItemType, sourceWorkItem, targetWorkItem, property, fieldName, convert, nullSub, fieldValue);
            }
            catch (NullReferenceException) when (fieldValue == null)
            {
                // This is most likely the cause of the field being null and the target property type not accepting nulls
                // For example: mapping null to an int instead of int?

                try
                {
                    Trace.TraceWarning(
                        "Could not map field '{0}' from type '{1}' to type '{2}'. Target '{2}.{3}' does not accept null values.",
                        fieldName,
                        sourceWorkItem.WorkItemType,
                        targetWorkItemType.Name,
                        $"{property.Name} ({property.PropertyType.FullName})");
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                    // Best effort
                }
            }
            catch (Exception e)
            {
                try
                {
                    Trace.TraceWarning(
                        "Could not map field '{0}' from type '{1}' to type '{2}.{3}'. {4}",
                        fieldName,
                        sourceWorkItem.WorkItemType,
                        targetWorkItemType.Name,
                        $"{property.Name} ({property.PropertyType.FullName})",
                        e.Message);
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                    // Best effort
                }
            }
        }

        protected internal override object GetFieldValue(Type targetWorkItemType, IWorkItem sourceWorkItem, string fieldName, PropertyInfo property)
        {
            try
            {
                return base.GetFieldValue(targetWorkItemType, sourceWorkItem, fieldName, property);
            }
            catch (DeniedOrNotExistException e)
            {
                // This is most likely caused by the field not being on the WIT
                // Frequently encountered when using a single entity to map different WITs with different sets of fields

                try
                {
                    Trace.TraceWarning(e.Message);
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                    // Best effort
                }
            }
            catch (Exception e)
            {
                try
                {
                    Trace.TraceWarning(
                        "Could not map field '{0}' from type '{1}' to type '{2}.{3}'. {4}",
                        fieldName,
                        sourceWorkItem.WorkItemType,
                        targetWorkItemType.Name,
                        $"{property.Name} ({property.PropertyType.FullName})",
                        e.Message);
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
                catch (Exception)
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                    // Best effort
                }
            }

            return null;
        }
    }
}