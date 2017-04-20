using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Qwiq.Mocks
{
    public class MockQueryByWiql : IQuery
    {
        private static readonly Regex EqualsRegex = new Regex(@"(?<direction>\[?(Source|Target)\]?)?\.?(?<field>\[?[A-Za-z0-9\. ]+\]?)\s?=\s?(?<value>[A-Za-z0-9\' \\ \.-]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex InRegex = new Regex(@"(?<direction>\[?(Source|Target)\]?)?\.?(?<field>\[?[A-Za-z0-9\. ]+\]?)\s?In\s?(?<value>\([A-Za-z0-9\'\\/, ]+\))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly IEnumerable<int> _ids;

        private readonly List<Tuple<string, string, string>> _parts;

        private readonly MockWorkItemStore _store;

        public MockQueryByWiql(IEnumerable<int> ids, string query, MockWorkItemStore store)
            : this(query, store)
        {
            _ids = ids;
        }

        public MockQueryByWiql(string query, MockWorkItemStore store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));

            _store = store;

            _parts = new List<Tuple<string, string, string>>();
            foreach (Match m in EqualsRegex.Matches(query))
            {
                var d = (m.Groups["direction"].Value ?? string.Empty)
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty);
                var f = m.Groups["field"].Value
                         .Replace("[", string.Empty)
                         .Replace("]", string.Empty);
                var v = m.Groups["value"]
                         .Value.Replace("'", string.Empty);

                var t = new Tuple<string, string, string>(d, f, v);
                _parts.Add(t);
            }

            foreach (Match m in InRegex.Matches(query))
            {
                var d = (m.Groups["direction"].Value ?? string.Empty)
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty);
                var f = m.Groups["field"]
                         .Value.Replace("[", string.Empty)
                         .Replace("]", string.Empty);
                var v = m.Groups["value"].Value
                         .Replace("'", string.Empty)
                         .Replace("(", string.Empty)
                         .Replace(")", string.Empty);

                var arrV = v.Split(',');
                foreach (var av in arrV)
                {
                    var t = new Tuple<string, string, string>(d, f, av.Trim());
                    _parts.Add(t);
                }
            }
        }

        public IWorkItemLinkTypeEndCollection GetLinkTypes()
        {
            // TODO: Limit IWorkItemLinkTypeEnds to the links contained in WIQL
            return _store.WorkItemLinkTypes.LinkTypeEnds;
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            var linkPredicates = _parts.Where(p => StringComparer.OrdinalIgnoreCase.Equals(CoreFieldRefNames.LinkType, p.Item2)).ToList();
            var fieldPredicateGroups = _parts.Except(linkPredicates).GroupBy(k => k.Item2, e => e).ToList();

            foreach (var li in _store.LinkInfo)
            {
                // If self link, return
                if (li.LinkType == null)
                {
                    yield return li;
                }
                else
                {
                    // Check the link predicate values for a match on the immutable name
                    foreach (var linkPredicate in linkPredicates)
                    {
                        if (!StringComparer.OrdinalIgnoreCase.Equals(linkPredicate.Item3, li.LinkType.ImmutableName)) continue;

                        // The link type immutable name matches. Check for additional predicates
                        if (!fieldPredicateGroups.Any())
                        {
                            // No additional predicates -- return the link
                            yield return li;
                        }
                        else
                        {
                            var match = true;
                            var source = _store.Query(li.SourceId);
                            var target = _store.Query(li.TargetId);

                            foreach (var fieldPredicateGroup in fieldPredicateGroups)
                            {
                                var matchAny = fieldPredicateGroup.Count() > 1;

                                if (matchAny)
                                {
                                    // Must match ANY (e.g. an IN)
                                    var directionGroups = fieldPredicateGroup.GroupBy(k => k.Item1, e => e).ToList();
                                    foreach (var directionGroup in directionGroups)
                                    {
                                        var workItem = "Source".Equals(directionGroup.Key, StringComparison.OrdinalIgnoreCase) ? source : target;
                                        match = directionGroup.Any(e => StringComparer.OrdinalIgnoreCase.Equals(e.Item3, workItem[e.Item2]?.ToString()));

                                        if (!match)
                                        {
                                            break;
                                        }
                                    }

                                    //foreach (var fieldPredicate in fieldPredicateGroup.Where(p => !string.IsNullOrEmpty(p.Item1)))
                                    //{
                                    //    var workItem = "Source".Equals(fieldPredicate.Item1, StringComparison.OrdinalIgnoreCase) ? source : target;
                                    //    match = MatchAggregate(match,  fieldPredicate, workItem);


                                    //}
                                }
                                else
                                {
                                    // There should be only one, so must match
                                    var fieldPredicate = fieldPredicateGroup.Single();
                                    var workItem = "Source".Equals(fieldPredicate.Item1, StringComparison.OrdinalIgnoreCase) ? source : target;
                                    match = MatchAggregate(match, fieldPredicate, workItem);
                                }

                                if (!match)
                                {
                                    break;
                                }
                            }

                            if (match)
                            {
                                yield return li;
                            }
                        }

                    }
                }
            }
        }

        private static bool MatchAggregate(bool m, Tuple<string, string, string> e, IWorkItemCore i)
        {
            m = m & StringComparer.OrdinalIgnoreCase.Equals(e.Item3, i[e.Item2]?.ToString());
            return m;
        }

        public IWorkItemCollection RunQuery()
        {
            return new WorkItemCollection(RunQueryImpl());
        }

        private IEnumerable<IWorkItem> RunQueryImpl()
        {
            if (_ids != null)
            {
                foreach (var id in _ids)
                {
                    if (_store._lookup.TryGetValue(id, out IWorkItem val))
                    {
                        yield return val;
                    }
                }
            }
            else if (_parts.Any())
            {
                var fieldPredicateGroups = _parts.GroupBy(k => k.Item2, e => e).ToList();
                foreach (var workItem in _store._lookup.Values)
                {
                    var match = true;

                    foreach (var fieldPredicateGroup in fieldPredicateGroups)
                    {
                        var matchAny = fieldPredicateGroup.Count() > 1;
                        if (matchAny)
                        {
                            // Must match ANY (e.g. an IN)
                            foreach (var fieldPredicate in fieldPredicateGroup.Where(p => !string.IsNullOrEmpty(p.Item1)))
                            {
                                match = MatchAggregate(match, fieldPredicate, workItem);

                                if (!match)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // There should be only one, so must match
                            var fieldPredicate = fieldPredicateGroup.Single();
                            match = MatchAggregate(match, fieldPredicate, workItem);
                        }

                        if (!match)
                        {
                            break;
                        }
                    }

                    if (match)
                    {
                        yield return workItem;
                    }
                }
            }
            else
            {
                foreach (var wi in _store._lookup.Values)
                {
                    yield return wi;
                }
            }
        }
    }
}