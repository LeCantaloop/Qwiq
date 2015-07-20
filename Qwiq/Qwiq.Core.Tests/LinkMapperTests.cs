using System;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Proxies;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Core.Tests
{
    public abstract class LinkMapperTests : ContextSpecification
    {
        internal LinkMapper Mapper;
        protected Tfs.WorkItem WorkItem;
        protected Tfs.WorkItemStore WorkItemStore;
        private IDisposable _shimsContext;

        public override void Given()
        {
            _shimsContext = ShimsContext.Create();

            var end = new Tfs.Fakes.ShimWorkItemLinkTypeEnd();

            var workItemLinkType = new Tfs.Fakes.ShimWorkItemLinkType
            {
                ReferenceNameGet = () => "System.LinkTypes.Hierarchy",
                ForwardEndGet = () => end,
                ReverseEndGet = () => end
            };

            var workItemLinkTypeCollection = new Tfs.Fakes.ShimWorkItemLinkTypeCollection
            {
                SystemCollectionsIEnumerableGetEnumerator = () => new[] {workItemLinkType}.GetEnumerator()
            };

            WorkItemStore = new Tfs.Fakes.ShimWorkItemStore
            {
                WorkItemLinkTypesGet = () => workItemLinkTypeCollection
            };

            WorkItem = new Tfs.Fakes.ShimWorkItem
            {
                StoreGet = () => WorkItemStore
            };

            Mapper = new LinkMapper();
        }

        public override void Cleanup()
        {
            _shimsContext.Dispose();
        }
    }

    [TestClass]
    public class given_a_tfs_hyperlink_and_a_LinkMapper_when_the_hyperlink_is_mapped : LinkMapperTests
    {
        private Tfs.Link _tfsLink;
        private IHyperlink _actual;
        private const string Domain = "http://domain.tld";

        public override void Given()
        {
            base.Given();
            _tfsLink = new Tfs.Hyperlink(Domain);
        }

        public override void When()
        {
            _actual = Mapper.Map(_tfsLink) as IHyperlink;
        }

        [TestMethod]
        public void the_LinkMapper_returns_a_corresponding_IHyperlink()
        {
            _actual.Location.ShouldEqual(Domain);
        }
    }

    [TestClass]
    public class given_an_ILink_and_a_LinkMapper_when_the_ILink_is_mapped : LinkMapperTests
    {
        private Tfs.Hyperlink _expected;
        private ILink _link;
        private Tfs.Hyperlink _actual;

        public override void Given()
        {
            base.Given();
            _expected = new Tfs.Hyperlink("http://domain.tld");
            _link = new HyperlinkProxy(_expected);
        }

        public override void When()
        {
            _actual = Mapper.Map(_link, WorkItem) as Tfs.Hyperlink;
        }

        [TestMethod]
        public void the_LinkMapper_returns_a_new_tfs_Link_with_the_same_fields_as_the_original_tfs_Link()
        {
            _actual.Location.ShouldEqual(_expected.Location);
        }
    }
}
