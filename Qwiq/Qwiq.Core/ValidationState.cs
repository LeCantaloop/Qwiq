namespace Microsoft.Qwiq
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// See <see cref="Microsoft.TeamFoundation.WorkItemTracking.Client.FieldStatus"/>
    /// </remarks>
    public enum ValidationState
    {
        Valid = 0,
        InvalidEmpty = 1,
        InvalidNotEmpty = 2,
        InvalidFormat = 3,
        InvalidListValue = 4,
        InvalidOldValue = 5,
        InvalidNotOldValue = 6,
        InvalidEmptyOrOldValue = 7,
        InvalidNotEmptyOrOldValue = 8,
        InvalidValueInOtherField = 9,
        InvalidValueNotInOtherField = 10,
        InvalidUnknown = 11,
        InvalidDate = 12,
        InvalidTooLong = 13,
        InvalidType = 14,
        InvalidComputedField = 15,
        InvalidPath = 16,
        InvalidCharacters = 17
    }
}

