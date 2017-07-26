using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public class NoExceptionAttributeMapperStrategy : AttributeMapperStrategy
    {
        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyInspector inspector) : base(inspector)
        {
        }

        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyInspector inspector, [NotNull] ITypeParser typeParser)
            : base(inspector, typeParser)
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
                catch (Exception)
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
                catch (Exception)
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
                catch (Exception)
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
                catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                    // Best effort
                }
            }

            return null;
        }
    }
}