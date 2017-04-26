using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Exceptions
{
    public class AggregateExceptionContextSpecification : ContextSpecification
    {
        protected IExceptionExploder Instance { get; set; }
        protected Exception Exception { get; set; }
        protected IEnumerable<Exception> Result { get; set; }

        public override void Given()
        {
            Instance = new AggregateExceptionExploder();
        }

        public override void When()
        {
            Result = Instance.Explode(Exception);
        }
    }
}