using System;
using System.Diagnostics;

namespace Microsoft.Qwiq.Mocks
{
    public class MockField : IField
    {
        private const int MaxStringLength = 255;

        private IRevisionInternal _revision;

        private object _value;

        public MockField(IFieldDefinition fieldDefinition)
        {
            FieldDefinition = fieldDefinition;
        }

        public MockField(
            IFieldDefinition fieldDefinition,
            object value,
            object originalValue = null,
            ValidationState validationState = ValidationState.Valid,
            bool isChangedByUser = true)
            : this(fieldDefinition)
        {
            Value = value;
            OriginalValue = originalValue;
            ValidationState = validationState;
            IsChangedByUser = isChangedByUser;
            IsEditable = true;
        }

        internal MockField(IRevisionInternal revision, IFieldDefinition definition)
            : this(definition)
        {
            Revision = revision;
        }

        public IFieldDefinition FieldDefinition { get; }

        public virtual int Id => FieldDefinition.Id;

        public bool IsChangedByUser { get; }

        public bool IsDirty { get; private set; }

        public bool IsEditable { get; }

        public bool IsRequired { get; }

        public virtual bool IsValid => ValidationState == ValidationState.Valid;

        public virtual string Name => FieldDefinition.Name;

        public object OriginalValue { get; set; }

        public virtual string ReferenceName => FieldDefinition.ReferenceName;

        public ValidationState ValidationState { get; private set; }

        public object Value
        {
            get => Revision != null ? Revision.GetCurrentFieldValue(FieldDefinition) : _value;
            set
            {
                if (Revision != null)
                {
                    Revision.SetFieldValue(FieldDefinition, value);
                }
                else
                {
                    Trace.TraceWarning(
                                       $"Value will be local to this field only. Use the {nameof(Revision)} property to attach a work item.");
                    _value = value;
                }

                IsDirty = true;

                if (OriginalValue != null && _value != null && _value.Equals(OriginalValue))
                {
                    IsDirty = false;
                    ValidationState = ValidationState.Valid;
                }

                switch (ValidationState)
                {
                    case ValidationState.InvalidNotEmpty:
                        if (string.IsNullOrEmpty(value as string)) ValidationState = ValidationState.Valid;
                        break;

                    case ValidationState.InvalidTooLong:
                        var str = value as string;
                        if (str != null && str.Length <= MaxStringLength) ValidationState = ValidationState.Valid;
                        break;

                    case ValidationState.Valid:
                        break;

                    case ValidationState.InvalidEmpty:
                        break;

                    case ValidationState.InvalidFormat:
                        break;

                    case ValidationState.InvalidListValue:
                        break;

                    case ValidationState.InvalidOldValue:
                        break;

                    case ValidationState.InvalidNotOldValue:
                        break;

                    case ValidationState.InvalidEmptyOrOldValue:
                        break;

                    case ValidationState.InvalidNotEmptyOrOldValue:
                        break;

                    case ValidationState.InvalidValueInOtherField:
                        break;

                    case ValidationState.InvalidValueNotInOtherField:
                        break;

                    case ValidationState.InvalidUnknown:
                        break;

                    case ValidationState.InvalidDate:
                        break;

                    case ValidationState.InvalidType:
                        break;

                    case ValidationState.InvalidComputedField:
                        break;

                    case ValidationState.InvalidPath:
                        break;

                    case ValidationState.InvalidCharacters:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal IRevisionInternal Revision
        {
            get => _revision;
            set
            {
                if (_revision != null && _revision != value) throw new InvalidOperationException("Revision already set");
                _revision = value;
                if (_value != null)
                {
                    _revision.SetFieldValue(FieldDefinition, _value);
                    _value = null;
                }
            }
        }
    }
}