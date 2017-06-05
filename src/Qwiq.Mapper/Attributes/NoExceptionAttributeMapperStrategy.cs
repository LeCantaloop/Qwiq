using System;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public class NoExceptionAttributeMapperStrategy : AttributeMapperStrategy
    {
        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyInspector inspector) : base(inspector)
        {
        }

        public NoExceptionAttributeMapperStrategy([NotNull] IPropertyInspector inspector, [NotNull] ITypeParser typeParser)
            :base(inspector, typeParser)
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
                catch (Exception)
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
                catch (Exception)
                {
                    // Best effort
                }
            }
        }
    }
}
