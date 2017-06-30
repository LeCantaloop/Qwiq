using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Microsoft.Qwiq.Mapper
{
    [ContractClassFor(typeof(IWorkItemMapper))]
    internal abstract class WorkItemMapperContract : IWorkItemMapper
    {
        public IEnumerable Create<T>(IEnumerable<IWorkItem> collection)
            where T : IIdentifiable<int?>, new()
        {
            Contract.Requires(collection != null);

            return default(IEnumerable);
        }

        /// <inheritdoc />
        IEnumerable<T> IWorkItemMapper.Create<T>(IEnumerable<IWorkItem> collection)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIdentifiable<int?>> Create(Type type, IEnumerable<IWorkItem> collection)
        {
            Contract.Requires(type != null);
            Contract.Requires(collection != null);

            return default(IEnumerable<IIdentifiable<int?>>);
        }

        /// <inheritdoc />
        public abstract T Default<T>()
            where T : new();

        /// <inheritdoc />
        public abstract IEnumerable<IWorkItemMapperStrategy> MapperStrategies { get; }
    }
}