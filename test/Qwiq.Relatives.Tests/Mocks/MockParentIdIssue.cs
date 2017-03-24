using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;

namespace Microsoft.Qwiq.Relatives.Tests.Mocks
{
    [WorkItemType("Mock Issue")]
    public class MockParentIdIssue : IIdentifiable
    {
        public static readonly IWorkItemType CustomWorkItemType = new MockWorkItemType("Mock Issue");

        public int ParentId { get; set; }

        public int Id { get; }
    }
}
