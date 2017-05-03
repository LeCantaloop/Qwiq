using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public abstract class WorkItemCommon : WorkItemCore, IWorkItemCommon, IEquatable<IWorkItemCommon>
    {
        protected internal WorkItemCommon()
        {
        }

        protected internal WorkItemCommon(IDictionary<string, object> fields)
            :base(fields)
        {
        }

        public virtual int? AreaId
        {
            get => GetValue<int?>(CoreFieldRefNames.AreaId);
            set => SetValue(CoreFieldRefNames.AreaId, value);
        }

        public virtual string AreaPath
        {
            get => GetValue<string>(CoreFieldRefNames.AreaPath);
            set => SetValue(CoreFieldRefNames.AreaPath, value);
        }

        public virtual string AssignedTo
        {
            get => GetValue<string>(CoreFieldRefNames.AssignedTo);
            set => SetValue(CoreFieldRefNames.AssignedTo, value);
        }

        public virtual int? AttachedFileCount => GetValue<int?>(CoreFieldRefNames.AttachedFileCount);

        public virtual string ChangedBy => GetValue<string>(CoreFieldRefNames.ChangedBy);

        public virtual DateTime? ChangedDate => GetValue<DateTime?>(CoreFieldRefNames.ChangedDate);

        public virtual string CreatedBy => GetValue<string>(CoreFieldRefNames.CreatedBy);

        public virtual DateTime? CreatedDate => GetValue<DateTime?>(CoreFieldRefNames.CreatedDate);

        public virtual string Description
        {
            get => GetValue<string>(CoreFieldRefNames.Description);
            set => SetValue(CoreFieldRefNames.Description, value);
        }

        public virtual int? ExternalLinkCount => GetValue<int?>(CoreFieldRefNames.ExternalLinkCount);

        public virtual string History
        {
            get => GetValue(CoreFieldRefNames.History) as string ?? string.Empty;
            set => SetValue(CoreFieldRefNames.History, value);
        }

        public virtual int? HyperlinkCount => GetValue<int?>(CoreFieldRefNames.HyperlinkCount);

        public virtual int? IterationId
        {
            get => GetValue<int?>(CoreFieldRefNames.IterationId);
            set => SetValue(CoreFieldRefNames.IterationId, value);
        }

        public virtual string IterationPath
        {
            get => GetValue<string>(CoreFieldRefNames.IterationPath);
            set => SetValue(CoreFieldRefNames.IterationPath, value);
        }

        public virtual int? RelatedLinkCount => GetValue<int?>(CoreFieldRefNames.RelatedLinkCount);

        public virtual DateTime? RevisedDate => GetValue<DateTime?>(CoreFieldRefNames.RevisedDate);

        public virtual string State
        {
            get => GetValue<string>(CoreFieldRefNames.State);
            set => SetValue(CoreFieldRefNames.State, value);
        }

        public virtual string Tags
        {
            get => GetValue<string>(CoreFieldRefNames.Tags);
            set => SetValue(CoreFieldRefNames.Tags, value);
        }

        public virtual string Title
        {
            get => GetValue<string>(CoreFieldRefNames.Title);
            set => SetValue(CoreFieldRefNames.Title, value);
        }

        public virtual int? Watermark => GetValue<int?>(CoreFieldRefNames.Watermark);

        public virtual string WorkItemType => GetValue<string>(CoreFieldRefNames.WorkItemType);

        public virtual string TeamProject => GetValue<string>(CoreFieldRefNames.TeamProject);

        public bool Equals(IWorkItemCommon other)
        {
            return NullableIdentifiableComparer.Default.Equals(this, other);
        }
    }
}