using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Exceptions
{
    /// <summary>
    /// Tests for AccessDeniedException.
    /// </summary>
    [TestClass]
    public class Given_AccessDeniedException_default_constructor : ContextSpecification
    {
        private AccessDeniedException _exception = null!;

        public override void When()
        {
            _exception = new AccessDeniedException();
        }

        [TestMethod]
        public void Then_message_is_default()
        {
            _exception.Message.ShouldNotBeNullOrEmpty();
        }

        [TestMethod]
        public void Then_inner_exception_is_null()
        {
            _exception.InnerException.ShouldBeNull();
        }
    }

    [TestClass]
    public class Given_AccessDeniedException_with_message : ContextSpecification
    {
        private AccessDeniedException _exception = null!;
        private const string TestMessage = "Access denied to work item";

        public override void When()
        {
            _exception = new AccessDeniedException(TestMessage);
        }

        [TestMethod]
        public void Then_message_is_set()
        {
            _exception.Message.ShouldBe(TestMessage);
        }

        [TestMethod]
        public void Then_inner_exception_is_null()
        {
            _exception.InnerException.ShouldBeNull();
        }
    }

    [TestClass]
    public class Given_AccessDeniedException_with_message_and_inner_exception : ContextSpecification
    {
        private AccessDeniedException _exception = null!;
        private InvalidOperationException _innerException = null!;
        private const string TestMessage = "Access denied to work item";

        public override void Given()
        {
            _innerException = new InvalidOperationException("Inner exception");
        }

        public override void When()
        {
            _exception = new AccessDeniedException(TestMessage, _innerException);
        }

        [TestMethod]
        public void Then_message_is_set()
        {
            _exception.Message.ShouldBe(TestMessage);
        }

        [TestMethod]
        public void Then_inner_exception_is_set()
        {
            _exception.InnerException.ShouldBe(_innerException);
        }
    }

    /// <summary>
    /// Tests for PageSizeRangeException.
    /// </summary>
    [TestClass]
    public class Given_PageSizeRangeException_default_constructor : ContextSpecification
    {
        private PageSizeRangeException _exception = null!;

        public override void When()
        {
            _exception = new PageSizeRangeException();
        }

        [TestMethod]
        public void Then_message_contains_expected_text()
        {
            _exception.Message.ShouldContain("PageSize");
            _exception.Message.ShouldContain("50");
            _exception.Message.ShouldContain("200");
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            _exception.Message.ShouldContain("TF237117");
        }
    }

    /// <summary>
    /// Tests for TransientException.
    /// </summary>
    [TestClass]
    public class Given_TransientException_with_message_and_inner_exception : ContextSpecification
    {
        private Qwiq.Exceptions.TransientException _exception = null!;
        private InvalidOperationException _innerException = null!;
        private const string TestMessage = "Transient error occurred";

        public override void Given()
        {
            _innerException = new InvalidOperationException("Inner exception");
        }

        public override void When()
        {
            _exception = new Qwiq.Exceptions.TransientException(TestMessage, _innerException);
        }

        [TestMethod]
        public void Then_message_is_set()
        {
            _exception.Message.ShouldBe(TestMessage);
        }

        [TestMethod]
        public void Then_inner_exception_is_set()
        {
            _exception.InnerException.ShouldBe(_innerException);
        }
    }

    /// <summary>
    /// Tests for DeniedOrNotExistException.
    /// </summary>
    [TestClass]
    public class Given_DeniedOrNotExistException_default_constructor : ContextSpecification
    {
        private DeniedOrNotExistException _exception = null!;

        public override void When()
        {
            _exception = new DeniedOrNotExistException();
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            _exception.Message.ShouldContain("TF237090");
        }

        [TestMethod]
        public void Then_message_contains_access_denied()
        {
            _exception.Message.ShouldContain("access is denied");
        }
    }

    [TestClass]
    public class Given_DeniedOrNotExistException_with_project_name : ContextSpecification
    {
        private DeniedOrNotExistException _exception = null!;
        private const string ProjectName = "TestProject";

        public override void When()
        {
            _exception = new DeniedOrNotExistException(ProjectName);
        }

        [TestMethod]
        public void Then_message_contains_project_name()
        {
            _exception.Message.ShouldContain(ProjectName);
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            _exception.Message.ShouldContain("TF26193");
        }
    }

    [TestClass]
    public class Given_DeniedOrNotExistException_with_message_and_inner : ContextSpecification
    {
        private DeniedOrNotExistException _exception = null!;
        private InvalidOperationException _innerException = null!;
        private const string TestMessage = "Access denied with inner";

        public override void Given()
        {
            _innerException = new InvalidOperationException("Inner error");
        }

        public override void When()
        {
            _exception = new DeniedOrNotExistException(TestMessage, _innerException);
        }

        [TestMethod]
        public void Then_message_is_set()
        {
            _exception.Message.ShouldBe(TestMessage);
        }

        [TestMethod]
        public void Then_inner_exception_is_set()
        {
            _exception.InnerException.ShouldBe(_innerException);
        }
    }

    [TestClass]
    public class Given_DeniedOrNotExistException_with_project_guid : ContextSpecification
    {
        private DeniedOrNotExistException _exception = null!;
        private Guid _projectGuid;

        public override void Given()
        {
            _projectGuid = Guid.NewGuid();
        }

        public override void When()
        {
            _exception = new DeniedOrNotExistException(_projectGuid);
        }

        [TestMethod]
        public void Then_message_contains_project_guid()
        {
            _exception.Message.ShouldContain(_projectGuid.ToString());
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            _exception.Message.ShouldContain("TF26193");
        }
    }

    /// <summary>
    /// Tests for FieldDefinitionNotExistException.
    /// </summary>
    [TestClass]
    public class Given_FieldDefinitionNotExistException_default_constructor : ContextSpecification
    {
        private FieldDefinitionNotExistException _exception = null!;

        public override void When()
        {
            _exception = new FieldDefinitionNotExistException();
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            _exception.Message.ShouldContain("TF26028");
        }

        [TestMethod]
        public void Then_message_contains_field_definition()
        {
            _exception.Message.ShouldContain("field definition");
        }
    }

    [TestClass]
    public class Given_FieldDefinitionNotExistException_with_field_name : ContextSpecification
    {
        private FieldDefinitionNotExistException _exception = null!;
        private const string FieldName = "System.CustomField";

        public override void When()
        {
            _exception = new FieldDefinitionNotExistException(FieldName);
        }

        [TestMethod]
        public void Then_message_contains_field_name()
        {
            _exception.Message.ShouldContain(FieldName);
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            _exception.Message.ShouldContain("TF26027");
        }
    }

    [TestClass]
    public class Given_FieldDefinitionNotExistException_with_message_and_inner_exception : ContextSpecification
    {
        private FieldDefinitionNotExistException _exception = null!;
        private InvalidOperationException _innerException = null!;
        private const string TestMessage = "Custom error message";

        public override void Given()
        {
            _innerException = new InvalidOperationException("Inner exception");
        }

        public override void When()
        {
            _exception = new FieldDefinitionNotExistException(TestMessage, _innerException);
        }

        [TestMethod]
        public void Then_message_is_set()
        {
            _exception.Message.ShouldBe(TestMessage);
        }

        [TestMethod]
        public void Then_inner_exception_is_set()
        {
            _exception.InnerException.ShouldBe(_innerException);
        }
    }

    /// <summary>
    /// Tests for WorkItemTypeDeniedOrNotExistException.
    /// </summary>
    [TestClass]
    public class Given_WorkItemTypeDeniedOrNotExistException_default_constructor : ContextSpecification
    {
        private WorkItemTypeDeniedOrNotExistException _exception = null!;

        public override void When()
        {
            _exception = new WorkItemTypeDeniedOrNotExistException();
        }

        [TestMethod]
        public void Then_inherits_from_DeniedOrNotExistException()
        {
            _exception.ShouldBeAssignableTo<DeniedOrNotExistException>();
        }

        [TestMethod]
        public void Then_message_contains_TF_error_code()
        {
            // Inherits default message from DeniedOrNotExistException
            _exception.Message.ShouldContain("TF237090");
        }
    }
}
