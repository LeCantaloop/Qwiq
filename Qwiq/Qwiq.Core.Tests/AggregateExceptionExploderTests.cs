using System;
using System.Collections.Generic;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Core.Tests
{
    public class AggregateExceptionExploderTests : ContextSpecification
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

    [TestClass]
    public class given_a_non_AggregateException_when_exploded : AggregateExceptionExploderTests
    {
        public override void Given()
        {
            base.Given();

            Exception = new ArgumentException();
        }

        [TestMethod]
        public void the_result_is_an_empty_enumerable()
        {
            Result.ShouldBeEmpty();
        }
    }
}
