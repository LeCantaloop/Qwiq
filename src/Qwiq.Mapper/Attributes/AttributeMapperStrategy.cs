using FastMember;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qwiq.Mapper.Attributes
{
    public class AttributeMapperStrategy : WorkItemMapperStrategyBase
    {
        [NotNull] private readonly IAnnotatedPropertyValidator _annotatedPropertyValidator;
        [NotNull] private readonly ITypeParser _typeParser;

        /// <summary>
        /// Creates a default instance of <see cref="AttributeMapperStrategy"/> with <see cref="PropertyReflector"/>.
        /// </summary>
        public AttributeMapperStrategy()
            : this(new PropertyReflector())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AttributeMapperStrategy"/> with the specified <paramref name="propertyReflector"/>.
        /// </summary>
        /// <param name="propertyReflector">An instance of <see cref="IPropertyReflector"/>.</param>
        public AttributeMapperStrategy([NotNull] IPropertyReflector propertyReflector)
            : this(new PropertyInspector(propertyReflector))
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AttributeMapperStrategy"/> with the specified <paramref name="inspector"/> and a default instance of <see cref="ITypeParser"/>.
        /// </summary>
        /// <param name="inspector">An instance of <see cref="IPropertyInspector"/>.</param>
        public AttributeMapperStrategy([NotNull] IPropertyInspector inspector)
            : this(inspector, TypeParser.Default)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AttributeMapperStrategy"/> with a <see cref="AnnotatedPropertyValidator"/> using the specified <paramref name="inspector"/> and <paramref name="typeParser"/>.
        /// </summary>
        /// <param name="inspector">An instance of <see cref="IPropertyInspector"/>.</param>
        /// <param name="typeParser">An instance of <see cref="ITypeParser"/>.</param>
        public AttributeMapperStrategy([NotNull] IPropertyInspector inspector, [NotNull] ITypeParser typeParser)
            : this(new AnnotatedPropertyValidator(inspector), typeParser)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AttributeMapperStrategy"/> with the specified <paramref name="annotatedPropertyValidator"/> and <paramref name="typeParser"/>.
        /// </summary>
        /// <param name="annotatedPropertyValidator">An instance of <see cref="IAnnotatedPropertyValidator"/>.</param>
        /// <param name="typeParser">An instance of <see cref="ITypeParser"/>.</param>
        public AttributeMapperStrategy([NotNull] IAnnotatedPropertyValidator annotatedPropertyValidator, [NotNull] ITypeParser typeParser)
        {
            _typeParser = typeParser ?? throw new ArgumentNullException(nameof(typeParser));
            _annotatedPropertyValidator = annotatedPropertyValidator ?? throw new ArgumentNullException(nameof(annotatedPropertyValidator));
        }

        public override void Map<T>(IDictionary<IWorkItem, T> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var targetWorkItemType = typeof(T);

            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;
                var targetWorkItem = workItemMapping.Value;

                MapImpl(targetWorkItemType, sourceWorkItem, targetWorkItem);
            }
        }

        public override void Map(Type targetWorkItemType, IDictionary<IWorkItem, IIdentifiable<int?>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;
                var targetWorkItem = workItemMapping.Value;

                MapImpl(targetWorkItemType, sourceWorkItem, targetWorkItem);
            }
        }

        protected internal virtual void AssignFieldValue(
            [NotNull] Type targetWorkItemType,
            [NotNull] IWorkItem sourceWorkItem,
            [NotNull] object targetWorkItem,
            [NotNull] PropertyInfo property,
            [NotNull] string fieldName,
            bool convert,
            [CanBeNull] object nullSub,
            [CanBeNull] object fieldValue)
        {
            // Coalesce fieldValue and nullSub

            if (fieldValue == null && nullSub != null)
            {
                fieldValue = nullSub;
            }
            else
            {
                if (fieldValue is string value && string.IsNullOrWhiteSpace(value))
                    fieldValue = nullSub;
            }

            var destType = property.PropertyType;

            if (fieldValue == null && destType.IsValueType)
            {
                // Value types do not accept null; don't do any work
                return;
            }

            if (fieldValue == null && destType.CanAcceptNull())
            {
                // Destination is a nullable or can take null; don't do any work
                return;
            }

            if (convert)
            {
                try
                {
                    fieldValue = _typeParser.Parse(destType, fieldValue);
                }
                catch (Exception e)
                {
                    var tm = new TypePair(sourceWorkItem, targetWorkItemType);
                    var pm = new PropertyMap(property, fieldName);
                    var message = $"Unable to convert field value on {sourceWorkItem.Id}.";
                    throw new AttributeMapException(message, e, tm, pm);
                }
            }

            var accessor = TypeAccessor.Create(targetWorkItemType, true);
            try
            {
                accessor[targetWorkItem, property.Name] = fieldValue;
            }
            catch (Exception e)
            {
                var tm = new TypePair(sourceWorkItem, targetWorkItemType);
                var pm = new PropertyMap(property, fieldName);
                var message = $"Unable to set field value on {sourceWorkItem.Id}.";
                throw new AttributeMapException(message, e, tm, pm);
            }
        }

        protected internal virtual object GetFieldValue(Type targetWorkItemType, IWorkItem sourceWorkItem, string fieldName, PropertyInfo property)
        {
            object fieldValue;
            try
            {
                fieldValue = sourceWorkItem[fieldName];
            }
            catch (DeniedOrNotExistException e)
            {
                var tm = new TypePair(sourceWorkItem, targetWorkItemType);
                var pm = new PropertyMap(property, fieldName);
                var message = $"Unable to get field value on {sourceWorkItem.Id}.";
                throw new AttributeMapException(message, e, tm, pm);
            }
            return fieldValue;
        }

        protected internal virtual void MapImpl(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem)
        {
            var validAnnotatedPropertyKeyPairs = _annotatedPropertyValidator.GetValidAnnotatedProperties(sourceWorkItem, targetWorkItemType);

            foreach (var pair in validAnnotatedPropertyKeyPairs)
            {
                var fieldDefinitionAttribute = pair.Value;
                if (fieldDefinitionAttribute == null) continue;

                var property = pair.Key;
                var fieldName = fieldDefinitionAttribute.FieldName;
                var convert = fieldDefinitionAttribute.RequireConversion;
                var nullSub = fieldDefinitionAttribute.NullSubstitute;
                var fieldValue = GetFieldValue(targetWorkItemType, sourceWorkItem, fieldName, property);

                AssignFieldValue(targetWorkItemType, sourceWorkItem, targetWorkItem, property, fieldName, convert, nullSub, fieldValue);
            }
        }
    }
}