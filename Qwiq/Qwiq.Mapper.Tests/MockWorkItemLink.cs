using System;

namespace Microsoft.IE.Qwiq.Mapper.Tests
{
    public class MockWorkItemLink : IWorkItemLink
    {
        public string Comment
        {
            get { throw new NotImplementedException(); }
        }

        public string AddedBy
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime AddedDate
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime? ChangedDate
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IWorkItemLinkTypeEnd LinkTypeEnd { get; set; }

        public int SourceId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int TargetId { get; set; }
    }
}