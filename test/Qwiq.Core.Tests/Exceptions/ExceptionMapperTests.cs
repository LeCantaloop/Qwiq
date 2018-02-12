using System;
using System.Collections.Generic;
using System.Linq;

using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Qwiq.UnitTests.Mocks;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq.Exceptions
{
    public class ExceptionMapperTests : ContextSpecification
    {
        protected IExceptionMapper ExceptionMapper { get; set; }
        protected IExceptionExploder[] ExceptionExploders { get; set; }
        protected IExceptionMapper[] ExceptionMappers { get; set; }
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
            ExceptionExploders = new IExceptionExploder[0];
            ExceptionMappers = new[] {new MockArgumentExceptionMapper()};
            Input = new ArgumentException(null, MockArgumentExceptionMapper.MockParamName);
            base.Given();
        }

        [TestMethod]
        public void exception_is_mapped_to_expected_type()
        {
            ActualResult.ShouldBeType<MockException>();
        }
    }

    [TestClass]
    public class given_an_exception_without_a_supported_mapper : ExceptionMapperTests
    {
        public override void Given()
        {
            ExceptionExploders = new IExceptionExploder[0];
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

    public abstract class VssExceptionMapperTests<T> : ExceptionMapperTests where T : Exception, new()
    {
        protected IEnumerable<int> HandledErrorCodes;

        public override void Given()
        {
            ExceptionExploders = new IExceptionExploder[0];
            ExceptionMappers = new[] {new MockVssExceptionMapper<T>(HandledErrorCodes.ToArray())};
            base.Given();
        }
    }

    [TestClass]
    public class given_a_VssExceptionMapper_with_an_exception_with_an_unhandled_error_code : VssExceptionMapperTests<MockException>
    {
        public override void Given()
        {
            HandledErrorCodes = new int[] {};
            Input = new VssServiceException("TFabcd: This is a sample exception");
            base.Given();
        }

        [TestMethod]
        public void the_original_exception_should_be_returned()
        {
            ActualResult.ShouldEqual(Input);
        }
    }

    [TestClass]
    public class given_a_VssExceptionMapper_with_an_exception_with_a_handled_error_code : VssExceptionMapperTests<MockException>
    {
        private readonly int _errorCode = 12345;
        public override void Given()
        {
            HandledErrorCodes = new[] {_errorCode};
            Input = new VssServiceException($"TF{_errorCode}: This is a sample exception");
            base.Given();
        }

        [TestMethod]
        public void the_exception_should_be_mapped()
        {
            ActualResult.ShouldBeType<MockException>();
        }
    }
}

