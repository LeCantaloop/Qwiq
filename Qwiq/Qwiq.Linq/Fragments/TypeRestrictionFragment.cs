using System;
using Microsoft.IE.Qwiq.Mapper;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class TypeRestrictionFragment : IFragment
    {
        private readonly IFieldMapper _fieldMapper;

        public TypeRestrictionFragment(IFieldMapper fieldMapper)
        {
            _fieldMapper = fieldMapper;
        }

        public string Get(Type queryType)
        {
            return "([Work Item Type] = '" + _fieldMapper.GetWorkItemType(queryType) + "')";
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
