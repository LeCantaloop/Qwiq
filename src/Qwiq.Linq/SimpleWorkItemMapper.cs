using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Mapper;

namespace Microsoft.Qwiq.Linq
{
    public class SimpleWorkItemMapper : IWorkItemMapper
    {
        public IEnumerable<T> Create<T>(IEnumerable<IWorkItem> collection) where T : IIdentifiable, new()
        {
            return collection.OfType<T>();
        }

        public IEnumerable<IIdentifiable> Create(Type type, IEnumerable<IWorkItem> collection)
        {
            return collection.OfType<IIdentifiable>();
        }

        public T Default<T>() where T : new()
        {
            return new T();
        }

        public IEnumerable<IWorkItemMapperStrategy> MapperStrategies { get; }
    }
}
