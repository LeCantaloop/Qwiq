using System;
using System.Collections.Generic;

using Qwiq.Tests.Common;

namespace Qwiq.Exceptions
{
    public class AggregateExceptionContextSpecification : ContextSpecification
    {
        protected IExceptionExploder Instance { get; set; } = null!;
        protected Exception Exception { get; set; } = null!;
        protected IEnumerable<Exception> Result { get; set; } = null!;

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