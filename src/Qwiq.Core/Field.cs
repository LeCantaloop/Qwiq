using System;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    internal class Field : IField
    {
        private readonly IRevisionInternal _revision;

        protected internal Field([NotNull] IRevisionInternal revision, [NotNull] IFieldDefinition fieldDefinition)
        {
            _revision = revision ?? throw new ArgumentNullException(nameof(revision));
            FieldDefinition = fieldDefinition ?? throw new ArgumentNullException(nameof(fieldDefinition));
        }

        /// <inheritdoc />
        public IFieldDefinition FieldDefinition { get; }

        public virtual int Id => FieldDefinition.Id;

        public virtual bool IsChangedByUser => throw new NotImplementedException();

        public virtual bool IsDirty => throw new NotImplementedException();

        public virtual bool IsEditable => throw new NotImplementedException();

        public virtual bool IsRequired => throw new NotImplementedException();

        public virtual bool IsValid => ValidationState == ValidationState.Valid;

        public virtual string Name => FieldDefinition.Name;

        public virtual object OriginalValue => throw new NotImplementedException();

        public virtual string ReferenceName => FieldDefinition.ReferenceName;

        public virtual ValidationState ValidationState => throw new NotImplementedException();

        public virtual object Value
        {
            get => _revision.GetCurrentFieldValue(FieldDefinition);
            set => _revision.SetFieldValue(FieldDefinition, value);
        }
    }
}