using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Mapper
{
    public class WorkItemMapper : IWorkItemMapper
    {
        public IEnumerable<IWorkItemMapperStrategy> MapperStrategies { get; }

        private delegate IIdentifiable ObjectActivator();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ObjectActivator> OptimizedCtorExpression = new ConcurrentDictionary<RuntimeTypeHandle, ObjectActivator>();

        public WorkItemMapper(IEnumerable<IWorkItemMapperStrategy> mapperStrategies)
        {
            MapperStrategies = mapperStrategies.ToList();
        }

        public T Default<T>() where T : new()
        {
            return new T();
        }

        public IEnumerable<T> Create<T>(IEnumerable<IWorkItem> collection) where T : IIdentifiable, new()
        {
            var workItemsToMap = new Dictionary<IWorkItem, T>();
            foreach (var item in collection)
            {
                workItemsToMap[item] = new T();
            }

            foreach (var strategy in MapperStrategies)
            {
                strategy.Map(workItemsToMap, this);
            }

            return workItemsToMap.Select(wi => wi.Value);
        }

        public IEnumerable<IIdentifiable> Create(Type type, IEnumerable<IWorkItem> collection)
        {
            // Activator.CreateInstance is about 0.2 ms per 1,000
            // Compiled expression is about 0.04 ms per 1,000
            var compiled = OptimizedCtorExpressionCache(type);
            var workItemsToMap = new Dictionary<IWorkItem, IIdentifiable>();
            foreach (var item in collection)
            {
                workItemsToMap[item] = compiled.Invoke();
            }


            foreach (var strategy in MapperStrategies)
            {
                strategy.Map(type, workItemsToMap, this);
            }

            return workItemsToMap.Select(wi => wi.Value);
        }

        private static ObjectActivator OptimizedCtorExpressionCache(Type type)
        {
            return OptimizedCtorExpression.GetOrAdd(
                type.TypeHandle,
                handle =>
                    {
                        // Find default ctor for target type
                        var ctor = type.GetConstructors().First();
                        var newExp = Expression.New(ctor);
                        var lambda = Expression.Lambda(typeof(ObjectActivator), newExp);
                        var compiled = (ObjectActivator)lambda.Compile();
                        return compiled;
                    });
        }
    }
}
