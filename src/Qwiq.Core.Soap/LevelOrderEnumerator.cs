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
            _current = null;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_current == null)
            {
                _current = _root;
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

            _current = _queue.Dequeue();
            ProcessCurrent();

            return true;
        }

        private void ProcessCurrent()
        {
            _currentGenerationCount--;
            if (_currentDepth >= _maxDepth) return;

            Debug.Assert(_current != null, nameof(_current) + " != null");

            foreach (Node child in _current!.ChildNodes)
            {
                _nextGenerationCount++;
                _queue.Enqueue(child);
            }
        }

        public void Reset()
        {
            _current = null;
            _queue.Clear();
            _currentGenerationCount = 1;
            _nextGenerationCount = 0;
            _currentDepth = 0;
        }

        private Node? _current;

        /// <summary>
        /// Gets the current node. Returns null only before first MoveNext or after Reset.
        /// </summary>
        /// <remarks>
        /// This is internal API and callers should only access Current after MoveNext returns true.
        /// </remarks>
        public Node Current => _current!;

        object IEnumerator.Current => Current;
    }
}