using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockField : IField
    {
        private const int MaxStringLength = 255;
        private object _value;

        public MockField(object value, object originalValue = null, ValidationState validationState = ValidationState.Valid, bool isChangedByUser = true)
        {
            Value = value;
            OriginalValue = originalValue;
            ValidationState = validationState;
            IsChangedByUser = isChangedByUser;
            IsEditable = true;
        }

        public int Id { get; set; }

        public bool IsDirty { get; private set; }
        public bool IsEditable { get; set; }

        public bool IsRequired { get; set; }

        public bool IsValid => ValidationState == ValidationState.Valid;

        public string Name { get; set; }

        public object OriginalValue { get; set; }
        public ValidationState ValidationState { get; private set; }
        public bool IsChangedByUser { get; }

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsDirty = true;

                if (OriginalValue != null && _value != null && _value.Equals(OriginalValue))
                {
                    IsDirty = false;
                    ValidationState = ValidationState.Valid;
                }

                switch (ValidationState)
                {
                    case ValidationState.InvalidNotEmpty:
                        if (string.IsNullOrEmpty(value as string))
                        {
                            ValidationState = ValidationState.Valid;
                        }
                        break;

                    case ValidationState.InvalidTooLong:
                        var str = value as string;
                        if (str != null && str.Length <= MaxStringLength)
                        {
                            ValidationState = ValidationState.Valid;
                        }
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
