using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Proxies;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Fakes;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Core.Tests
{
    public abstract class LinkCollectionTests : ContextSpecification
    {
        protected LinkCollectionProxy LinkCollection;
        protected IList<Hyperlink> Links;

        private IDisposable _shimsContext;

        public override void Given()
        {
            _shimsContext = ShimsContext.Create();

            var linksCollection = new ShimLinkCollection
            {
                CountGet = () => Links.Count(),
                GetItemInt32 = index => Links[index],
                ItemGetInt32 = index => Links[index]
            };
            new ShimVariableSizeList(linksCollection)
            {
                GetEnumerator = () => Links.GetEnumerator()
            };
            var workItem = new ShimWorkItem
            {
                LinksGet = () => linksCollection
            };
            LinkCollection = new LinkCollectionProxy(workItem);
        }

        public override void Cleanup()
        {
            _shimsContext.Dispose();
        }
    }

    [TestClass]
    public class given_a_LinkCollectionProxy_with_two_items_when_CopyTo_is_invoked : LinkCollectionTests
    {
        private ILink[] _actual;
        private const string Comment1 = "comment1";
        private const string Comment2 = "comment2";

        public override void Given()
        {
            Links = new[]
            {
                new Hyperlink("http://domain.tld")
                {
                    Comment = Comment1
                },
                new Hyperlink("http://domain2.tld")
                {
                    Comment = Comment2
                }
            };
            base.Given();
        }

        public override void When()
        {
            _actual = LinkCollection.ToArray();
        }

        [TestMethod]
        public void a_new_collection_with_two_items_is_created()
        {
            _actual.Count().ShouldEqual(Links.Count());
        }

        [TestMethod]
        public void the_comment_of_the_first_item_is_the_same()
        {
            _actual.First().Comment.ShouldEqual(Comment1);
        }

        [TestMethod]
        public void the_comment_of_the_second_item_is_the_same()
        {
            _actual.Last().Comment.ShouldEqual(Comment2);
        }
    }

    [TestClass]
    public class given_a_LinkCollectionProxy_with_two_items_when_CopyTo_is_invoked_with_an_offset_index_of_one :
        LinkCollectionTests
    {
        private ILink[] _actual;
        readonly Hyperlink _hyperlink1 = new Hyperlink("http://domain.tld");
        readonly Hyperlink _hyperlink2 = new Hyperlink("http://domain2.tld");

        public override void Given()
        {
            Links = new[]
            {
                _hyperlink1,
                _hyperlink2
            };
            base.Given();
        }

        public override void When()
        {
            _actual = new ILink[1];
            LinkCollection.CopyTo(_actual, 1);
        }

        [TestMethod]
        public void the_copied_collection_contains_only_the_second_element()
        {
            (_actual.Single() as IHyperlink).Location.ShouldEqual(_hyperlink2.Location);
        }
    }
}
