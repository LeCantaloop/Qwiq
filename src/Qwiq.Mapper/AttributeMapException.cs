using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper
{
    [DebuggerDisplay("{SourceField} -> {DestinationProperty.Name}")]
    public struct PropertyMap
    {
        public PropertyMap(PropertyInfo destinationProperty, string sourceField)
        {
            DestinationProperty = destinationProperty;
            SourceField = sourceField;
        }

        public PropertyInfo DestinationProperty { get; }
        public string SourceField { get; }
    }

    [DebuggerDisplay("{Source.WorkItemType} -> {Destination.Name}")]
    public struct TypePair
    {
        public TypePair(IWorkItem source, Type destination)
        {
            Source = source;
            Destination = destination;
        }

        public Type Destination { get; }
        public IWorkItem Source { get; }
    }

    public class AttributeMapException : ApplicationException
    {
        private readonly string _message;

        public AttributeMapException()
        {
        }

        public AttributeMapException([CanBeNull] string message)
            : base(message) => _message = message;

        public AttributeMapException([CanBeNull] string message, [CanBeNull] Exception innerException)
            : base(message, innerException) => _message = message;

        public AttributeMapException([CanBeNull] string message, [CanBeNull] Exception innerException, TypePair typePair, PropertyMap properties)

            : this(message, innerException)
        {
            Types = typePair;
            PropertyMap = properties;
        }

        public override string Message
        {
            get
            {
                var message = _message ?? string.Empty;
                var newLine = Environment.NewLine;

                if (Types?.Source != null && Types?.Destination != null)
                {
                    message = $"{message}{newLine}{newLine}Mapping types:";
                    message += $"{newLine}{Types?.Source.WorkItemType} -> {Types?.Destination.FullName}";
                }

                if (PropertyMap?.DestinationProperty != null && !string.IsNullOrEmpty(PropertyMap?.SourceField))
                {
                    message = $"{message}{newLine}{newLine}Property:";
                    message += $"{newLine}{PropertyMap?.SourceField} -> {PropertyMap?.DestinationProperty.Name}";
                }

                return message;
            }
        }

        public PropertyMap? PropertyMap { get; set; }
        public TypePair? Types { get; set; }
    }
}