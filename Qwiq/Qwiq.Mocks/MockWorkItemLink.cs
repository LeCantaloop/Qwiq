using System;

namespace Microsoft.Qwiq.Mapper.Tests
{
    public class MockWorkItemLink : IRelatedLink
    {
        public string Comment { get; }
        public BaseLinkType BaseType => BaseLinkType.RelatedLink;
        public int RelatedWorkItemId { get; set; }
        public IWorkItemLinkTypeEnd LinkTypeEnd { get; set; }
        public string LinkSubType { get; }
    }
}
