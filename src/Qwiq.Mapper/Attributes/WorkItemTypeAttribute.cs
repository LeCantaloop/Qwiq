using System;

namespace Qwiq.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class WorkItemTypeAttribute : Attribute
    {
        private readonly string _type;

        public WorkItemTypeAttribute(string type)
        {
            _type = type;
        }

        public string GetTypeName()
        {
            return _type;
        }
    }
}

