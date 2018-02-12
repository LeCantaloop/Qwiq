using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Qwiq.Linq
{
    [ContractClassFor(typeof(IFieldMapper))]
    internal abstract class FieldMapperContract : IFieldMapper
    {
        public string GetFieldName(Type type, string propertyName)
        {
            Contract.Requires(type != null);
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            return default(string);
        }

        public IEnumerable<string> GetFieldNames(Type type)
        {
            Contract.Requires(type != null);

            return default(IEnumerable<string>);
        }

        public IEnumerable<string> GetWorkItemType(Type type)
        {
            Contract.Requires(type != null);

            return default(IEnumerable<string>);
        }
    }
}