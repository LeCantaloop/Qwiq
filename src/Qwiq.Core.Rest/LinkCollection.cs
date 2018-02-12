using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class LinkCollection : ReadOnlyObjectWithNameCollection<ILink>, ICollection<ILink>
    {
        internal LinkCollection([CanBeNull] List<WorkItemRelation> relations, [NotNull] Func<string, IWorkItemLinkType> linkFunc)
        {
            Contract.Requires(linkFunc != null);

            if (relations == null) return;
            if (linkFunc == null) throw new ArgumentNullException(nameof(linkFunc));

            for (var i = 0; i < relations.Count; i++)
            {
                var relation = relations[i];
                var rel = string.Intern(relation.Rel);

                if (CoreLinkTypeReferenceNames.Related.Equals(rel, StringComparison.OrdinalIgnoreCase))
                {
                    // Last part of the Url is the ID
                    var lte = linkFunc(rel).ForwardEnd;
                    var l = new RelatedLink(ExtractId(relation.Url), lte, ExtractComment(relation.Attributes));
                    Add(l);
                }
                else if ("Hyperlink".Equals(rel, StringComparison.OrdinalIgnoreCase))
                {
                    var l = new Hyperlink(relation.Url, ExtractComment(relation.Attributes));
                    Add(l);
                }
                else if ("ArtifactLink".Equals(rel, StringComparison.OrdinalIgnoreCase))
                {
                    const string Name = "name";
                    var l = new ExternalLink(
                                             relation.Url,
                                             ExtractProperty(relation.Attributes, Name),
                                             ExtractComment(relation.Attributes));
                    Add(l);
                }
                else if ("AttachedFile".Equals(relation.Rel, StringComparison.OrdinalIgnoreCase))
                {
                }
                else
                {
                    if (relation.Rel.IndexOf('-') > -1)
                    {
                        var arrRel = rel.Split('-');
                        var lt = linkFunc(arrRel[0]);
                        var lte = rel.Equals(lt.ForwardEnd.ImmutableName, StringComparison.OrdinalIgnoreCase)
                                      ? lt.ForwardEnd
                                      : lt.ReverseEnd;

                        Debug.Assert(
                                     StringComparer.OrdinalIgnoreCase.Equals(lte.ImmutableName, relation.Rel),
                                     "LinkTypeEnd.ImmutableName != WorkItemRelation.Rel");

                        var l = new RelatedLink(ExtractId(relation.Url), lte, ExtractComment(relation.Attributes));
                        Add(l);
                    }
                    else
                    {
                        throw new NotSupportedException($"{rel} is not supported.");
                    }
                }
            }
        }

        public bool IsReadOnly => true;

        public void CopyTo(ILink[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new ArgumentException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException(nameof(array));
            foreach (var value in this) array.SetValue(value, arrayIndex++);
        }

        void ICollection<ILink>.Add(ILink item)
        {
            throw new NotSupportedException();
        }

        void ICollection<ILink>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<ILink>.Remove(ILink item)
        {
            throw new NotSupportedException();
        }

        private static string ExtractComment(IDictionary<string, object> relationAttributes)
        {
            const string Comment = "comment";
            return ExtractProperty(relationAttributes, Comment);
        }

        private static int ExtractId(string uri)
        {
            var arr = uri.Split('/');
            return Convert.ToInt32(arr.Last());
        }

        private static string ExtractProperty(IDictionary<string, object> relationAttributes, string property)
        {
            relationAttributes.TryGetValue(property, out object val);
            return val?.ToString();
        }
    }
}