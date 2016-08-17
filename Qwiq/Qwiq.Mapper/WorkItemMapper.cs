﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.IE.Qwiq.Mapper
{
    public class WorkItemMapper : IWorkItemMapper
    {
        private readonly IEnumerable<IWorkItemMapperStrategy> _mapperStrategies;
        private delegate object ObjectActivator();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ObjectActivator> OptimizedCtorExpression = new ConcurrentDictionary<RuntimeTypeHandle, ObjectActivator>();

        public WorkItemMapper(IEnumerable<IWorkItemMapperStrategy> mapperStrategies)
        {
            _mapperStrategies = mapperStrategies.ToList();
        }

        public T Default<T>() where T : new()
        {
            return new T();
        }

        public IEnumerable<T> Create<T>(IEnumerable<IWorkItem> collection) where T : new()
        {
            var type = typeof(T);
            var workItemsToMap = collection.Select(wi => new KeyValuePair<IWorkItem, T>(wi, new T())).ToList();
            foreach (var strategy in _mapperStrategies)
            {
                strategy.Map(type, workItemsToMap, this);
            }

            return workItemsToMap.Select(wi => wi.Value);
        }

        public IEnumerable Create(Type type, IEnumerable<IWorkItem> collection)
        {
            return ParseWorkItems(type, collection);
        }

        private IEnumerable ParseWorkItems(Type type, IEnumerable<IWorkItem> collection)
        {
            // Activator.CreateInstance is SLOW
            //  About 0.2 ms per 1,000
            // Compiled expression is 0.04 ms per 1,000
            var compiled = OptimizedCtorExpressionCache(type);


            var workItemsToMap = collection.Select(wi => new KeyValuePair<IWorkItem, object>(wi, compiled.Invoke())).ToList();

            foreach (var strategy in _mapperStrategies)
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