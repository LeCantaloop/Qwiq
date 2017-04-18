using System;

namespace Microsoft.Qwiq
{
    internal class Field : IField
    {
        private readonly IRevisionInternal _revision;

        private readonly IFieldDefinition _fieldDefinition;

        protected internal Field(IRevisionInternal revision, IFieldDefinition fieldDefinition)
        {
            _revision = revision ?? throw new ArgumentNullException(nameof(revision));
            _fieldDefinition = fieldDefinition ?? throw new ArgumentNullException(nameof(fieldDefinition));
        }

        public virtual bool IsValid => ValidationState == ValidationState.Valid;

        public virtual string Name => _fieldDefinition.Name;

        public virtual string ReferenceName => _fieldDefinition.ReferenceName;

        public virtual object OriginalValue => throw new NotImplementedException();

        public virtual ValidationState ValidationState => throw new NotImplementedException();

        public virtual bool IsChangedByUser => throw new NotImplementedException();

        public virtual object Value
        {
            get => _revision.GetCurrentFieldValue(_fieldDefinition);
            set => _revision.SetFieldValue(_fieldDefinition, value);
        }

        public virtual int Id => _fieldDefinition.Id;

        public virtual bool IsDirty => throw new NotImplementedException();

        public virtual bool IsEditable => throw new NotImplementedException();

        public virtual bool IsRequired => throw new NotImplementedException();
    }
}