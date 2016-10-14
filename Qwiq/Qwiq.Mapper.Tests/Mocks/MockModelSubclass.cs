using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("Baz")]
    public class MockModelSubclass : MockModel
    {
        public override string StringField { get { return "42"; } }
    }
}

