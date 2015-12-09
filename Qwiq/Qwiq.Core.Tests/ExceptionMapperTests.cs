using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Core.Tests.Mocks;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Core.Tests
{
    public class ExceptionMapperTests : ContextSpecification
    {
        protected IExceptionMapper ExceptionMapper { get; set; }
        protected IEnumerable<IExceptionExploder> ExceptionExploders { get; set; }
        protected IEnumerable<IExceptionMapper> ExceptionMappers { get; set; }
        protected Exception Input { get; set; }
        protected Exception ActualResult { get; set; }

        public override void Given()
        {
            ExceptionMapper = new ExceptionMapper(ExceptionExploders, ExceptionMappers);
        }

        public override void When()
        {
            ActualResult = ExceptionMapper.Map(Input);
        }
    }

    [TestClass]
    public class given_an_exception_with_a_supported_mapper : ExceptionMapperTests
    {
        public override void Given()
        {
            ExceptionExploders = Enumerable.Empty<IExceptionExploder>();
            ExceptionMappers = new[] {new MockArgumentExceptionMapper()};
            Input = new ArgumentException(null, MockArgumentExceptionMapper.MockParamName);
            base.Given();
        }

        [TestMethod]
        public void exception_is_mapped_to_expected_type()
        {
            ActualResult.ShouldBeOfType<MockException>();
        }
    }

    [TestClass]
    public class given_an_exception_without_a_supported_mapper : ExceptionMapperTests
    {
        public override void Given()
        {
            ExceptionExploders = Enumerable.Empty<IExceptionExploder>();
            ExceptionMappers = new[] { new MockArgumentExceptionMapper() };
            Input = new ArgumentNullException();
            base.Given();
        }

        [TestMethod]
        public void exception_is_not_changed()
        {
            ActualResult.ShouldEqual(Input);
        }
    }

    [TestClass]
    public class given_a_InnerExceptionExploder_and_an_exception_with_multiple_levels_of_inner_exceptions_when_exploded : ExceptionMapperTests
    {
        private MockArgumentExceptionMapper CountingMapper { get; set; }

        public override void Given()
        {
            CountingMapper = new MockArgumentExceptionMapper();
            ExceptionExploders = new[] {new InnerExceptionExploder()};
            ExceptionMappers = new[] { CountingMapper };
            Input = new ArgumentException("One", new ArgumentException("Two", new ArgumentException("Three")));
            base.Given();
        }

        [TestMethod]
        public void the_argument_mapper_should_be_called_three_times()
        {
            CountingMapper.ExecutionCount.ShouldEqual(3);
        }
    }
}
