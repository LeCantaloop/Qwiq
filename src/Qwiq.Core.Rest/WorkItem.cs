using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;


using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Qwiq.Client.Rest
{
    internal class WorkItem : Qwiq.WorkItem
    {
        private readonly Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;
        private readonly Func<string, IWorkItemLinkType> _linkFunc;
        private IFieldCollection? _fields;
        private LinkCollection? _links;

        private Uri? _uri;

        public WorkItem(
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item,
            IWorkItemType wit,
            Func<string, IWorkItemLinkType> linkFunc)
            : base(wit)
        {
            Contract.Requires(item != null);
            Contract.Requires(wit != null);
            Contract.Requires(linkFunc != null);
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _linkFunc = linkFunc ?? throw new ArgumentNullException(nameof(linkFunc));
            Url = _item.Url;
            _uri = new Uri(_item.Url, UriKind.Absolute);
        }

        public WorkItem(
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item,
            Lazy<IWorkItemType> wit,
            Func<string, IWorkItemLinkType> linkFunc)
            : base(wit)
        {
            Contract.Requires(item != null);
            Contract.Requires(wit != null);
            Contract.Requires(linkFunc != null);
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _linkFunc = linkFunc ?? throw new ArgumentNullException(nameof(linkFunc));
            Url = _item.Url;
        }

        public override int AttachedFileCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.AttachedFileCount);
                if (!fv.HasValue)
                {
                    var cnt = Attachments.Count();
                    SetValue(CoreFieldRefNames.AttachedFileCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override int ExternalLinkCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.ExternalLinkCount);
                if (!fv.HasValue)
                {
                    var cnt = Links.Count(p => p.BaseType == BaseLinkType.ExternalLink);
                    SetValue(CoreFieldRefNames.ExternalLinkCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override IFieldCollection Fields => _fields
                                                   ?? (_fields = new FieldCollection(
                                                                                      this,
                                                                                      Type.FieldDefinitions,
                                                                                      (r, d) => new Field(r, d)));

        public override int HyperlinkCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.HyperlinkCount);
                if (!fv.HasValue)
                {
                    var cnt = Links.Count(p => p.BaseType == BaseLinkType.Hyperlink);
                    SetValue(CoreFieldRefNames.HyperlinkCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override int Id => _item.Id.GetValueOrDefault(0);

        public override ICollection<ILink> Links => _links
                                                    ?? (_links = new LinkCollection((List<WorkItemRelation>)_item.Relations, _linkFunc));

        /// <inheritdoc />
        public override int RelatedLinkCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.ExternalLinkCount);
                if (!fv.HasValue)
                {
                    var cnt = Links.Count(p => p.BaseType == BaseLinkType.RelatedLink);
                    SetValue(CoreFieldRefNames.ExternalLinkCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override int Rev => _item.Rev.GetValueOrDefault(0);

        public override string Url { get; }

        protected override object? GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            _item.Fields.TryGetValue(name, out object? value);

            // Convert IdentityRef objects to a string format compatible with SOAP
            // IdentityRef.ToString() returns the type name, not a useful value
            if (value is IdentityRef identityRef)
            {
                value = FormatIdentityRef(identityRef);
            }

#if DEBUG
            Trace.WriteLine($"Get \'{name}\': {value.ToUsefulString()}");
#endif
            return value;
        }

        /// <summary>
        /// Formats an IdentityRef object to a string compatible with SOAP identity field format.
        /// </summary>
        /// <param name="identityRef">The identity reference from the REST API.</param>
        /// <returns>A formatted string like "Display Name &lt;unique@name.com&gt;" or just the display name.</returns>
        private static string? FormatIdentityRef(IdentityRef? identityRef)
        {
            if (identityRef == null) return null;

            var displayName = identityRef.DisplayName;
            var uniqueName = identityRef.UniqueName;

            if (string.IsNullOrEmpty(displayName))
            {
                return uniqueName;
            }

            if (string.IsNullOrEmpty(uniqueName) || displayName.Equals(uniqueName, StringComparison.OrdinalIgnoreCase))
            {
                return displayName;
            }

            // Format as "Display Name <unique@name.com>" to match SOAP combo string format
            return $"{displayName} <{uniqueName}>";
        }

        protected override void SetValue(string name, object? value)
        {
            if (string.IsNullOrEmpty(name)) return;

            _item.Fields[name] = value;
#if DEBUG
            Trace.WriteLine($"Set \'{name}\' to {value.ToUsefulString()}");
#endif
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_item != null);
        }
    }
}