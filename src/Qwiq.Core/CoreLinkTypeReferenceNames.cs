using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Qwiq
{
  public static class CoreLinkTypeReferenceNames
  {
    public static readonly string Related = "System.LinkTypes.Related";
    public static readonly string Hierarchy = "System.LinkTypes.Hierarchy";
    public static readonly string Dependency = "System.LinkTypes.Dependency";
    public static readonly string Duplicate = "System.LinkTypes.Duplicate";
    /// <summary>
    /// private static array that keeps all reference names of corefield
    /// so that caller can do enumeration easily
    /// </summary>
    private static readonly string[] _all = {
                                        Related,
                                        Hierarchy,
                                        Dependency,
                                        Duplicate
                                      };

    /// <summary>Returns the set of all core link types.</summary>
    public static IEnumerable<string> All => _all;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct CoreLinkTypes
  {
    public const int Related = 1;
    public const int Parent = 2;
    public const int Child = -2;
    public const int Predecessor = 3;
    public const int Successor = -3;
  }

  public static class CoreFieldRefNames
  {
    private static string[] m_all = new string[29]
                                      {
                                        "System.AuthorizedDate",
                                        "System.AreaId",
                                        "System.AreaPath",
                                        "System.ExternalLinkCount",
                                        "System.AssignedTo",
                                        "System.AttachedFileCount",
                                        "System.AuthorizedAs",
                                        "System.ChangedBy",
                                        "System.ChangedDate",
                                        "System.CreatedBy",
                                        "System.CreatedDate",
                                        "System.Description",
                                        "System.History",
                                        "System.HyperLinkCount",
                                        "System.Id",
                                        "System.IterationId",
                                        "System.IterationPath",
                                        "System.Links.LinkType",
                                        "System.NodeName",
                                        "System.Reason",
                                        "System.RelatedLinkCount",
                                        "System.Rev",
                                        "System.RevisedDate",
                                        "System.State",
                                        "System.TeamProject",
                                        "System.Title",
                                        "System.Watermark",
                                        "System.WorkItemType",
                                        "System.IsDeleted"
                                      };
    public const string AreaId = "System.AreaId";
    public const string AreaPath = "System.AreaPath";
    public const string AssignedTo = "System.AssignedTo";
    public const string AttachedFileCount = "System.AttachedFileCount";
    public const string AuthorizedAs = "System.AuthorizedAs";
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
    public const string IterationId = "System.IterationId";
    public const string IterationPath = "System.IterationPath";
    public const string LinkType = "System.Links.LinkType";
    public const string NodeName = "System.NodeName";
    public const string Reason = "System.Reason";
    public const string RelatedLinkCount = "System.RelatedLinkCount";
    public const string Rev = "System.Rev";
    public const string RevisedDate = "System.RevisedDate";
    public const string State = "System.State";
    public const string AuthorizedDate = "System.AuthorizedDate";
    public const string TeamProject = "System.TeamProject";
    public const string Tags = "System.Tags";
    public const string Title = "System.Title";
    public const string WorkItemType = "System.WorkItemType";
    public const string Watermark = "System.Watermark";
    public const string IsDeleted = "System.IsDeleted";

    public static IEnumerable<string> All
    {
      get
      {
        return (IEnumerable<string>)CoreFieldRefNames.m_all;
      }
    }
  }
}
