using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal class LevelOrderEnumerator : IEnumerator<Node>
    {
        private readonly Queue<Node> _queue;
        private int _currentGenerationCount;
        private int _nextGenerationCount;
        private readonly Node _root;
        private readonly int? _maxDepth;
        private int _currentDepth;

        public LevelOrderEnumerator(Node root, int? maxDepth = null)
        {
            _root = root;
            _maxDepth = maxDepth;
            _currentDepth = 0;
            _queue = new Queue<Node>();
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

            foreach (Node child in Current.ChildNodes)
            {
                _nextGenerationCount++;
                _queue.Enqueue(child);
            }
        }

        public void Reset()
        {
            Current = null;
        }

        public Node Current { get; private set; }

        object IEnumerator.Current => Current;
    }
}