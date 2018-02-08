using System;
using System.Collections.Generic;

namespace Qwiq.Mocks
{
    public class WorkItemLinkGenerator<T> : WorkItemGenerator<T>
        where T : IWorkItem
    {
        private readonly Func<IWorkItemLinkTypeEnd, int, int, ILink> _linkFunc;

        private readonly IWorkItemLinkType _linkType;

        public WorkItemLinkGenerator(
            Func<T> createFunc,
            IWorkItemLinkType linkType,
            Func<IWorkItemLinkTypeEnd, int, int, ILink> linkFunc)
            : this(createFunc, linkType, linkFunc, null)
        {
        }

        public WorkItemLinkGenerator(
            Func<T> createFunc,
            IWorkItemLinkType linkType,
            Func<IWorkItemLinkTypeEnd, int, int, ILink> linkFunc,
            IEnumerable<string> propertiesToSkip = null)
            : base(createFunc, propertiesToSkip)
        {
            _linkFunc = linkFunc ?? throw new ArgumentNullException(nameof(linkFunc));
            _linkType = linkType ?? throw new ArgumentNullException(nameof(linkType));
        }

        protected override object GetRandomValue(T instance, string propertyName, Type propertyType)
        {
            switch (propertyType.ToString())
            {
                case "System.Collections.Generic.ICollection`1[Microsoft.Qwiq.ILink]":
                    var retval = new List<ILink>();

                    if (Randomizer.Instance.ShouldEnter())
                    {
                        // Create a random set of child links
                        var childLinkCount = Randomizer.Instance.Next(1, 3);
                        for (var i = 0; i < childLinkCount; i++)
                        {
                            var s = instance.Id;
                            var t = Randomizer.Instance.NextSystemId(s, int.MaxValue);

                            while (s == t)
                            {
                                t = Randomizer.Instance.NextSystemId(int.MaxValue);
                            }

                            retval.Add(_linkFunc(_linkType.ForwardEnd, s, t));
                        }
                    }

                    // Randomly create a parent link
                    if (Randomizer.Instance.ShouldEnter())
                    {
                        var s = instance.Id;
                        // The target is constrained here to create higher density observed in actual work item structures
                        var t = Randomizer.Instance.NextSystemId(1, 36);

                        while (s == t)
                        {
                            t = Randomizer.Instance.NextSystemId(1, 36);
                        }

                        retval.Add(_linkFunc(_linkType.ReverseEnd, s, t));
                    }

                    //TODO: Generate external links (Branch, Build, Fixed in Changeset, Fixed in Commit, Hyperlink, Pull Request
                    return retval;

                default:
                    return base.GetRandomValue(instance, propertyName, propertyType);
            }
        }
    }
}