using System.Collections.Generic;
using System.Linq;

namespace Qwiq.WorkItemStore
{
    public abstract class LargeHierarchyContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        internal const string WIQL = @"
SELECT [System.Id], [System.Title]
FROM WorkItemLinks
WHERE
    Source.[System.Id] IN (10726623)
    AND [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'
mode(Recursive)
";

        public override void When()
        {
            RestResult.Links = TimedAction(() => RestResult.WorkItemStore.QueryLinks(WIQL).ToList(), "REST", "QueryLinks - WIQL");
            var hs = new HashSet<int>(RestResult.Links.SelectMany(dl => new[] { dl.TargetId, dl.SourceId }));
            hs.Remove(0);
            RestResult.WorkItems = TimedAction(
                                               () => RestResult.WorkItemStore.Query(hs),
                                               "REST",
                                               "Query - IDs");

            

            SoapResult.Links = TimedAction(() => SoapResult.WorkItemStore.QueryLinks(WIQL).ToList(), "SOAP", "QueryLinks - WIQL");
            hs = new HashSet<int>(SoapResult.Links.SelectMany(dl => new[] { dl.TargetId, dl.SourceId }));
            hs.Remove(0);

            SoapResult.WorkItems = TimedAction(
                                               () => SoapResult.WorkItemStore.Query(hs),
                                               "SOAP",
                                               "Query - IDs");
        }
    }
}