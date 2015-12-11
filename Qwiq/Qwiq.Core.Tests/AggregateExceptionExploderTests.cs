using System;
using System.Collections.Generic;
using System.Linq;
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

    [TestClass]
    public class given_an_AggregateException_containing_AggregateExceptions_when_exploded : AggregateExceptionExploderTests
    {
        private IEnumerable<Exception> ExpectedExceptions { get; set; }

        public override void Given()
        {
            base.Given();

            var exception1 = new ArgumentException();
            var exception2 = new ArgumentException();

            ExpectedExceptions = new[] {exception1, exception2};

            Exception = new AggregateException(exception1, new AggregateException(exception2, new AggregateException()));
        }

        [TestMethod]
        public void the_result_should_have_the_expected_exceptions()
        {
            Result.ShouldContainOnly(ExpectedExceptions);
        }
    }
}
