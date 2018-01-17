using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal static class WorkItemClassificationNodeCollectionBuilder
    {
        private static readonly Regex AreaRegex = new Regex("/classificationNodes/Areas/(?<path>.*)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex IterationRegex = new Regex("/classificationNodes/Iterations/(?<path>.*)", RegexOptions.Compiled | RegexOptions.Singleline);

        public static async Task<IWorkItemClassificationNodeCollection<int>> BuildAsync(Task<WorkItemClassificationNode> collection)
        {
            var n = await collection.ConfigureAwait(false);

            // SOAP client does not return the root (e.g. "\"), so return the root's children to match implementation
            return new WorkItemClassificationNodeCollection<int>(NewMethod(n.Children, n.Name));
        }

        private static IEnumerable<IWorkItemClassificationNode<int>> NewMethod(IEnumerable<WorkItemClassificationNode> collection, string rootPath)
        {
            foreach (var n in collection)
            {
                var e = new LevelOrderEnumerator(n);
                while (e.MoveNext())
                {
                    Debug.Assert(e.Current != null, "e.Current != null");

                    var t = e.Current.StructureType == TreeNodeStructureType.Area
                            ? NodeType.Area : e.Current.StructureType == TreeNodeStructureType.Iteration ? NodeType.Iteration : NodeType.None;

                    var u = DecodeUrlString(e.Current.Url);

                    var m = (t == NodeType.Area ? AreaRegex : IterationRegex).Match(u);
                    var p = m.Groups["path"].Value;
                    p = rootPath + "\\" + p.Replace("%20", " ").Replace("/", "\\");

                    yield return new WorkItemClassificationNode<int>(
                        e.Current.Id,
                        t,
                        p,
                        new Uri(e.Current.Url)
                    );
                }
            }
        }

        private static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }
    }
}