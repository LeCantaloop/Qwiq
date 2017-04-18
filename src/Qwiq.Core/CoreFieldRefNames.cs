using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public static class CoreFieldRefNames
    {
        public const string AreaId = "System.AreaId";

        public const string AreaPath = "System.AreaPath";

        public const string AssignedTo = "System.AssignedTo";

        public const string AttachedFileCount = "System.AttachedFileCount";

        public const string AuthorizedAs = "System.AuthorizedAs";

        public const string AuthorizedDate = "System.AuthorizedDate";

        public const string BoardColumn = "System.BoardColumn";

        public const string BoardColumnDone = "System.BoardColumnDone";

        public const string BoardLane = "System.BoardLane";

        public const string ChangedBy = "System.ChangedBy";

        public const string ChangedDate = "System.ChangedDate";

        public const string CreatedBy = "System.CreatedBy";

        public const string CreatedDate = "System.CreatedDate";

        public const string Description = "System.Description";

        public const string ExternalLinkCount = "System.ExternalLinkCount";

        public const string History = "System.History";

        public const string HyperLinkCount = "System.HyperLinkCount";

        public const string Id = "System.Id";

        public const string IsDeleted = "System.IsDeleted";

        public const string IterationId = "System.IterationId";

        public const string IterationPath = "System.IterationPath";

        public const string LinkType = "System.Links.LinkType";

        public const string NodeName = "System.NodeName";

        public const string Reason = "System.Reason";

        public const string RelatedLinkCount = "System.RelatedLinkCount";

        public const string Rev = "System.Rev";

        public const string RevisedDate = "System.RevisedDate";

        public const string State = "System.State";

        public const string Tags = "System.Tags";

        public const string TeamProject = "System.TeamProject";

        public const string Title = "System.Title";

        public const string Watermark = "System.Watermark";

        public const string WorkItemType = "System.WorkItemType";

        public static IEnumerable<string> All => NameLookup.Keys.Except(new[] { LinkType });

        public static IReadOnlyDictionary<string, int> CoreFieldIdLookup { get; } =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { AreaId, (int)CoreField.AreaId },
                    { AreaPath, (int)CoreField.AreaPath },
                    { AssignedTo, (int)CoreField.AssignedTo },
                    { AttachedFileCount, (int)CoreField.AttachedFileCount },
                    { AuthorizedAs, (int)CoreField.AuthorizedAs },
                    { AuthorizedDate, (int)CoreField.AuthorizedDate },
                    { BoardColumn, (int)CoreField.BoardColumn },
                    { BoardColumnDone, (int)CoreField.BoardColumnDone },
                    { BoardLane, (int)CoreField.BoardLane },
                    { ChangedBy, (int)CoreField.ChangedBy },
                    { ChangedDate, (int)CoreField.ChangedDate },
                    { CreatedBy, (int)CoreField.CreatedBy },
                    { CreatedDate, (int)CoreField.CreatedDate },
                    { Description, (int)CoreField.Description },
                    { ExternalLinkCount, (int)CoreField.ExternalLinkCount },
                    { History, (int)CoreField.History },
                    { HyperLinkCount, (int)CoreField.HyperLinkCount },
                    { Id, (int)CoreField.Id },
                    { IterationId, (int)CoreField.IterationId },
                    { IterationPath, (int)CoreField.IterationPath },
                    { LinkType, (int)CoreField.LinkType },
                    { NodeName, (int)CoreField.NodeName },
                    { Reason, (int)CoreField.Reason },
                    { RelatedLinkCount, (int)CoreField.RelatedLinkCount },
                    { Rev, (int)CoreField.Rev },
                    { RevisedDate, (int)CoreField.RevisedDate },
                    { State, (int)CoreField.State },
                    { Tags, (int)CoreField.Tags },
                    { TeamProject, (int)CoreField.TeamProject },
                    { Title, (int)CoreField.Title },
                    { Watermark, (int)CoreField.Watermark },
                    { WorkItemType, (int)CoreField.WorkItemType }
                };

        /// <summary>
        ///     Given a reference name from <see cref="CoreFieldRefNames" />, get the corresponding friendly name.
        /// </summary>
        public static IReadOnlyDictionary<string, string> NameLookup { get; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { AreaId, "Area ID" },
                    { AreaPath, "Area Path" },
                    { AssignedTo, "Assigned To" },
                    { AttachedFileCount, "Attached File Count"},
                    { AuthorizedAs, "Authorized As" },
                    { AuthorizedDate, "Authorized Date" },
                    { BoardColumn, "Board Column" },
                    { BoardColumnDone, "Board Column Done" },
                    { BoardLane, "Board Lane" },
                    { ChangedBy, "Changed By" },
                    { ChangedDate, "Changed Date" },
                    { CreatedBy, "Created By" },
                    { CreatedDate, "Created Date" },
                    { Description, "Description" },
                    { ExternalLinkCount, "External Link Count" },
                    { History, "History" },
                    { HyperLinkCount, "Hyperlink Count" },
                    { Id, "ID" },
                    { IterationId, "Iteration ID" },
                    { IterationPath, "Iteration Path" },
                    { LinkType, "Link Type" },
                    { NodeName, "Node Name" },
                    { Reason, "Reason" },
                    { RelatedLinkCount, "Related Link Count" },
                    { Rev, "Rev" },
                    { RevisedDate, "Revised Date" },
                    { State, "State" },
                    { Tags, "Tags" },
                    { TeamProject, "Team Project" },
                    { Title, "Title" },
                    { Watermark, "Watermark" },
                    { WorkItemType, "Work Item Type" }
                };

        /// <summary>
        ///     Given a friendly name for one of <see cref="CoreFieldRefNames" />, get the corresponding reference name.
        /// </summary>
        public static IReadOnlyDictionary<string, string> ReferenceNameLookup { get; } = NameLookup.ToDictionary(
            k => k.Value,
            e => e.Key,
            StringComparer.OrdinalIgnoreCase);
    }
}