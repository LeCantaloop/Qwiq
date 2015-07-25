using System;
using Microsoft.IE.Qwiq.Mapper;

namespace Microsoft.IE.Qwiq.Linq.Fragments
{
    internal class SelectFragment : IFragment
    {
        private readonly IFieldMapper _fieldMapper;

        public SelectFragment(IFieldMapper fieldMapper)
        {
            _fieldMapper = fieldMapper;
        }

        public string Get(Type queryType)
        {
            return "SELECT " + String.Join(", ", _fieldMapper.GetFieldNames(queryType)) + " FROM WorkItems";
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
