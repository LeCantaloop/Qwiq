using System;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Field : Qwiq.Field
    {
        private readonly Tfs.Field _field;

        internal Field(Tfs.Field field)
            :base(
                 ExceptionHandlingDynamicProxyFactory.Create<IRevisionInternal>(new WorkItem(field?.WorkItem)),
                 ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(new FieldDefinition(field?.FieldDefinition)))
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public override int Id => _field.Id;

        public override bool IsChangedByUser => _field.IsChangedByUser;

        public override bool IsDirty => _field.IsDirty;

        public override bool IsEditable => _field.IsEditable;

        public override bool IsRequired => _field.IsRequired;

        public override bool IsValid => _field.IsValid;

        public override string Name => _field.Name;

        public override string ReferenceName => _field.ReferenceName;

        public override object OriginalValue
        {
            get => _field.OriginalValue;
            set => _field.Value = value;
        }

        public override ValidationState ValidationState => (ValidationState)(int)_field.Status;

        public override object Value
        {
            get => _field.Value;
            set => _field.Value = value;
        }
    }
}