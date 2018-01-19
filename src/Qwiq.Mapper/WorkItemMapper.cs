using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mapper
{
    public class WorkItemMapper : IWorkItemMapper
    {
        public IEnumerable<IWorkItemMapperStrategy> MapperStrategies { get; }

        private delegate IIdentifiable<int?> ObjectActivator();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ObjectActivator> OptimizedCtorExpression = new ConcurrentDictionary<RuntimeTypeHandle, ObjectActivator>();

        public WorkItemMapper([NotNull] params IWorkItemMapperStrategy[] mapperStrategies)
        {
            Contract.Requires(mapperStrategies != null);

            if (mapperStrategies == null) throw new ArgumentNullException(nameof(mapperStrategies));
            if (mapperStrategies.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(mapperStrategies));

            MapperStrategies = mapperStrategies;
        }

        public WorkItemMapper([NotNull] IEnumerable<IWorkItemMapperStrategy> mapperStrategies)
        {
            Contract.Requires(mapperStrategies != null);

            MapperStrategies = mapperStrategies?.ToList() ?? throw new ArgumentNullException(nameof(mapperStrategies));
        }

        public T Default<T>() where T : new()
        {
            return new T();
        }

        public IEnumerable<T> Create<T>(IEnumerable<IWorkItem> collection) where T : IIdentifiable<int?>, new()
        {
            var workItemsToMap = new Dictionary<IWorkItem, T>(Comparer.WorkItem);
            foreach (var item in collection)
            {
                workItemsToMap[item] = new T();
            }

            foreach (var strategy in MapperStrategies)
            {
                strategy.Map(workItemsToMap, this);
            }

            foreach (var wi in workItemsToMap)
            {
                yield return wi.Value;
            }
        }

        public IEnumerable<IIdentifiable<int?>> Create(Type type, IEnumerable<IWorkItem> collection)
        {
            // Activator.CreateInstance is about 0.2 ms per 1,000
            // Compiled expression is about 0.04 ms per 1,000
            var compiled = OptimizedCtorExpressionCache(type);
            var workItemsToMap = new Dictionary<IWorkItem, IIdentifiable<int?>>();
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

        [NotNull]
        private static ObjectActivator OptimizedCtorExpressionCache([NotNull] Type type)
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<ObjectActivator>() != null);

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
