using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockField : IField
    {
        private readonly IRevisionInternal _revision;

        private readonly IFieldDefinition _fieldDefinition;

        private const int MaxStringLength = 255;

        private object _value;

        private ValidationState _validationState;

        private bool _isDirty;

        public MockField(IFieldDefinition fieldDefinition)
        {
            _fieldDefinition = fieldDefinition;
        }

        internal MockField(IRevisionInternal revision, IFieldDefinition definition)
            :this(definition)
        {
            _revision = revision;
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
            _validationState = validationState;
            IsChangedByUser = isChangedByUser;
            IsEditable = true;
        }

        [Obsolete("This method has been deprecated and will be removed in a future version.")]
        public MockField(
            object value,
            object originalValue = null,
            ValidationState validationState = ValidationState.Valid,
            bool isChangedByUser = true)
            :this(MockFieldDefinition.Create(Guid.NewGuid().ToString("N")), value, originalValue, validationState, isChangedByUser)
        {
        }



        public bool IsChangedByUser { get; }

        public bool IsDirty => _isDirty;

        public bool IsEditable { get;  }

        public bool IsRequired { get; }

        public virtual bool IsValid => ValidationState == ValidationState.Valid;

        public virtual string Name => _fieldDefinition.Name;

        public virtual string ReferenceName => _fieldDefinition.ReferenceName;

        public virtual int Id => _fieldDefinition.Id;

        public object OriginalValue { get; set; }

        public ValidationState ValidationState => _validationState;

        public object Value
        {
            get => _revision != null ? _revision.GetCurrentFieldValue(_fieldDefinition) : _value;
            set
            {
                if (_revision != null) _revision.SetFieldValue(_fieldDefinition, value);
                else _value = value;

                _isDirty = true;

                if (OriginalValue != null && _value != null && _value.Equals(OriginalValue))
                {
                    _isDirty = false;
                    _validationState = ValidationState.Valid;
                }

                switch (ValidationState)
                {
                    case ValidationState.InvalidNotEmpty:
                        if (string.IsNullOrEmpty(value as string)) _validationState = ValidationState.Valid;
                        break;

                    case ValidationState.InvalidTooLong:
                        var str = value as string;
                        if (str != null && str.Length <= MaxStringLength) _validationState = ValidationState.Valid;
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
    }
}