using System;
using Microsoft.IE.Qwiq.Mapper;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class MemberFragment : IFragment
    {
        private readonly IFieldMapper _fieldMapper;

        private string ParameterName { get; set; }

        internal MemberFragment(IFieldMapper fieldMapper, string parameterName)
        {
            _fieldMapper = fieldMapper;
            ParameterName = parameterName;
        }

        public string Get(Type queryType)
        {
            return _fieldMapper.GetFieldName(queryType, ParameterName);
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
