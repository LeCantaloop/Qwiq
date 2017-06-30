using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Exceptions
{
    [TestClass]
    public class given_a_non_AggregateException_when_exploded : AggregateExceptionContextSpecification
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
    public class given_an_AggregateException_containing_AggregateExceptions_when_exploded : AggregateExceptionContextSpecification
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

