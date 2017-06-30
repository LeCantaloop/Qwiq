using System;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class Field : Qwiq.Field
    {
        internal Tfs.Field NativeField { get; }

        internal Field(Tfs.Field field)
            :base(
                 ExceptionHandlingDynamicProxyFactory.Create<IRevisionInternal>(new WorkItem(field?.WorkItem)),
                 ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(new FieldDefinition(field?.FieldDefinition)))
        {
            NativeField = field ?? throw new ArgumentNullException(nameof(field));
        }

        public override int Id => NativeField.Id;

        public override bool IsChangedByUser => NativeField.IsChangedByUser;

        public override bool IsDirty => NativeField.IsDirty;

        public override bool IsEditable => NativeField.IsEditable;

        public override bool IsRequired => NativeField.IsRequired;

        public override bool IsValid => NativeField.IsValid;

        public override string Name => NativeField.Name;

        public override string ReferenceName => NativeField.ReferenceName;

        public override object OriginalValue => NativeField.OriginalValue;

        public override ValidationState ValidationState => (ValidationState)(int)NativeField.Status;

        public override object Value
        {
            get => NativeField.Value;
            set => NativeField.Value = value;
        }
    }
}