using System;
using System.Linq;
using System.Reflection;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Mapper.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Mapper.Tests
{
    public abstract class CachingPropertyReflectorContext : ContextSpecification
    {
        protected IPropertyReflector Reflector;
        protected MockPropertyReflector InnerReflector;
        protected Type Type;
        protected PropertyInfo Property;

        public override void Given()
        {
            InnerReflector = new MockPropertyReflector();
            Reflector = new CachingPropertyReflector(InnerReflector);

            Type = typeof(MockModel);
            Property = typeof(MockModel).GetProperties().First();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_an_attribute_is_retrieved_twice : CachingPropertyReflectorContext
    {
        public override void When()
        {
            Reflector.GetAttribute(Type, Property);
            Reflector.GetAttribute(Type, Property);
        }

        [TestMethod]
        public void the_inner_PropertyReflectors_GetAttribute_is_never_called()
        {
            InnerReflector.GetAttributeCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void the_inner_PropertyReflectors_GetAttributes_is_only_called_once()
        {
            InnerReflector.GetCustomAttributesCallCount.ShouldEqual(1);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_properties_are_retrieved_twice : CachingPropertyReflectorContext
    {
        public override void When()
        {
            Reflector.GetProperties(Type);
            Reflector.GetProperties(Type);
        }

        [TestMethod]
        public void the_inner_PropertyReflectors_GetProperties_is_only_called_once()
        {
            InnerReflector.GetPropertiesCallCount.ShouldEqual(1);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_all_custom_attributes_are_retrieved_twice : CachingPropertyReflectorContext
    {
        public override void When()
        {
            Reflector.GetCustomAttributes(Property);
            Reflector.GetCustomAttributes(Property);
        }

        [TestMethod]
        public void the_inner_PropertyReflectors_GetCustomAttributesFiltered_is_never_called()
        {
            InnerReflector.GetCustomAttributesFilteredCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void the_inner_PropertyReflectors_GetCustomAttributes_is_only_called_once()
        {
            InnerReflector.GetCustomAttributesCallCount.ShouldEqual(1);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_custom_attributes_are_retrieved_twice : CachingPropertyReflectorContext
    {
        public override void When()
        {
            Reflector.GetCustomAttributes(Property, Type);
            Reflector.GetCustomAttributes(Property, Type);
        }

        [TestMethod]
        public void the_inner_PropertyReflectors_GetCustomAttributesFiltered_is_never_called()
        {
            InnerReflector.GetCustomAttributesFilteredCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void the_innterPropertyReflectors_GetCustomAttributes_is_only_called_once()
        {
            InnerReflector.GetCustomAttributesCallCount.ShouldEqual(1);
        }
    }
}
