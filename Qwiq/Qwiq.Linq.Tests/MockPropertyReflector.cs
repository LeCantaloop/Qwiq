using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Linq.Tests
{
    public class MockPropertyReflector : IPropertyReflector
    {
        public int GetAttributeCallCount { get; private set; }
        public int GetCustomAttributesCallCount { get; private set; }
        public int GetPropertiesCallCount { get; private set; }

        public int GetCustomAttributesFilteredCallCount { get; private set; }

        public IEnumerable<PropertyInfo> GetProperties(Type workItemType)
        {
            GetPropertiesCallCount += 1;
            return Enumerable.Empty<PropertyInfo>();
        }


        public IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property)
        {
            GetCustomAttributesCallCount += 1;
            return Enumerable.Empty<Attribute>();
        }

        public IEnumerable<Attribute> GetCustomAttributes(PropertyInfo property, Type attributeType)
        {
            GetCustomAttributesFilteredCallCount += 1;
            return Enumerable.Empty<Attribute>();
        }

        public Attribute GetAttribute(Type type, PropertyInfo property)
        {
            GetAttributeCallCount += 1;
            return null;
        }
    }
}
