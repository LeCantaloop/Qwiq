﻿using System;

namespace Microsoft.IE.Qwiq.Linq.Tests.Mocks
{
    public class MockWorkItemLinkInfo : IWorkItemLinkInfo
    {
        public bool IsLocked
        {
            get { throw new NotImplementedException(); }
        }

        public int LinkTypeId
        {
            get { throw new NotImplementedException(); }
        }

        public int SourceId { get; set; }

        public int TargetId { get; set; }
    }
}