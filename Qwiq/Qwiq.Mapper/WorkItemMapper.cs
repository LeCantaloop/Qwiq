using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mapper
{
    public class WorkItemMapper : IWorkItemMapper
    {
        private readonly IFieldMapper _fieldMapper;
        private readonly IEnumerable<IWorkItemMapperStrategy> _mapperStrategies;

        public WorkItemMapper(IFieldMapper fieldMapper, IEnumerable<IWorkItemMapperStrategy> mapperStrategies)
        {
            _fieldMapper = fieldMapper;
            _mapperStrategies = mapperStrategies;
        }

        public T Default<T>() where T : new()
        {
            return new T();
        }

        public IEnumerable<T> Create<T>(IEnumerable<IWorkItem> collection) where T : new()
        {
            return from workItem in collection
                   where workItem.Type.Name == _fieldMapper.GetWorkItemType(typeof(T)) // Only create the item if the type we're trying to make matches the TFS type. Otherwise, the fields won't map correctly and we'll throw an exception
                   select ParseWorkItem<T>(workItem);
        }

        public IEnumerable Create(Type type, IEnumerable<IWorkItem> collection)
        {
            return from workItem in collection
                   where workItem.Type.Name == _fieldMapper.GetWorkItemType(type) // Only create the item if the type we're trying to make matches the TFS type. Otherwise, the fields won't map correctly and we'll throw an exception
                   select ParseWorkItem(type, workItem);
        }

        private T ParseWorkItem<T>(IWorkItem workItem) where T : new()
        {
            return (T)ParseWorkItem(typeof(T), workItem);
        }

        private object ParseWorkItem(Type type, IWorkItem workItem)
        {
            var issue = Activator.CreateInstance(type);

            foreach (var strategy in _mapperStrategies)
            {
                strategy.Map(type, workItem, issue, this);
            }

            return issue;
        }
    }
}