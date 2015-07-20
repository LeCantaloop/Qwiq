using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("Baz")]
    class MockModelSubclass : MockModel
    {
        public override string StringField { get { return "42"; } }
    }
}
