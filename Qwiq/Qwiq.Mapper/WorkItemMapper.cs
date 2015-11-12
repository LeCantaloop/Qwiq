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
            return ParseWorkItems(typeof(T), collection).Cast<T>();
        }

        public IEnumerable Create(Type type, IEnumerable<IWorkItem> collection)
        {
            return ParseWorkItems(type, collection);
        }

        private IEnumerable ParseWorkItems(Type type, IEnumerable<IWorkItem> collection)
        {
            var expectedWorkItemType = _fieldMapper.GetWorkItemType(type);
            var workItemsToMap =
                collection.Where(wi => wi.Type.Name == expectedWorkItemType)
                    .Select(wi => new WorkItemMapping(wi, Activator.CreateInstance(type))).ToList();

            foreach (var strategy in _mapperStrategies)
            {
                strategy.Map(type, workItemsToMap, this);
            }

            return workItemsToMap.Select(wi => wi.MappedWorkItem);
        }
    }
}