using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Mocks;

namespace Microsoft.IE.Qwiq.Relatives.Tests.Mocks
{
    [WorkItemType("Mock Issue")]
    public class MockParentIdIssue : IIdentifiable
    {
        public static readonly IWorkItemType CustomWorkItemType = new MockWorkItemType
        {
            Name = "Mock Issue"
        };

        public int ParentId { get; set; }

        public int Id { get; }
    }
}