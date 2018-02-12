using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class LevelOrderEnumerator : IEnumerator<WorkItemClassificationNode>
    {
        private readonly Queue<WorkItemClassificationNode> _queue;
        private int _currentGenerationCount;
        private int _nextGenerationCount;
        private readonly WorkItemClassificationNode _root;
        private readonly int? _maxDepth;
        private int _currentDepth;

        public LevelOrderEnumerator(WorkItemClassificationNode root, int? maxDepth = null)
        {
            _root = root;
            _maxDepth = maxDepth;
            _currentDepth = 0;
            _queue = new Queue<WorkItemClassificationNode>();
            _currentGenerationCount = 1;
            _nextGenerationCount = 0;
            Current = null;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (Current == null)
            {
                Current = _root;
                ProcessCurrent();

                return true;
            }

            if (_queue.Count == 0)
            {
                return false;
            }

            if (_currentGenerationCount == 0)
            {
                _currentDepth++;
                _currentGenerationCount = _nextGenerationCount;
                _nextGenerationCount = 0;
            }

            Current = _queue.Dequeue();
            ProcessCurrent();

            return true;
        }

        private void ProcessCurrent()
        {
            _currentGenerationCount--;
            if (_currentDepth >= _maxDepth) return;

            Debug.Assert(Current != null, nameof(Current) + " != null");

            if (Current.Children == null) return;

            foreach (var child in Current.Children)
            {
                _nextGenerationCount++;
                _queue.Enqueue(child);
            }
        }

        public void Reset()
        {
            Current = null;
        }

        public WorkItemClassificationNode Current { get; private set; }

        object IEnumerator.Current => Current;
    }
}