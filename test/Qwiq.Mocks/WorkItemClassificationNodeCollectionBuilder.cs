using System;
using System.Collections.Generic;

namespace Qwiq.Mocks
{
    internal static class WorkItemClassificationNodeCollectionBuilder
    {
        public static IWorkItemClassificationNodeCollection<int> Build(NodeType type)
        {
            return new WorkItemClassificationNodeCollection<int>(BuildNode(type));
        }

        private static IEnumerable<IWorkItemClassificationNode<int>> BuildNode(NodeType type)
        {
            string path = "\\";
            for (int j = 1; j < 4; j++)
            {
                path += j + "\\";
                yield return new WorkItemClassificationNode<int>(j, type, path, new Uri("http://localhost/nodes/" + j));
            }
        }
    }
}