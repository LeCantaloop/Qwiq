using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal static class WorkItemClassificationNodeCollectionBuilder
    {
        public static IWorkItemClassificationNodeCollection<int> Build(NodeCollection collection)
        {
            return new WorkItemClassificationNodeCollection<int>(EnumerateNodeCollection(collection));
        }

        private static IEnumerable<IWorkItemClassificationNode<int>> EnumerateNodeCollection(NodeCollection collection)
        {
            foreach (Node n in collection)
            {
                var e = new LevelOrderEnumerator(n);
                while (e.MoveNext())
                {
                    yield return BuildNode(e);
                }
            }
        }

        private static IWorkItemClassificationNode<int> BuildNode([NotNull] LevelOrderEnumerator e)
        {
            Debug.Assert(e.Current != null, "e.Current != null");

            return new WorkItemClassificationNode<int>(
                e.Current.Id,
                e.Current.IsAreaNode ? NodeType.Area : e.Current.IsIterationNode ? NodeType.Iteration : NodeType.None,
                e.Current.Path,
                e.Current.Uri
            );
        }
    }
}