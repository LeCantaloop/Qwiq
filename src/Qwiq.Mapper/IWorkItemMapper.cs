using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mapper
{
    [ContractClass(typeof(WorkItemMapperContract))]
    public interface IWorkItemMapper
    {
        ///  <summary>
        ///  Converts a set of WorkItemss into a
        ///  subclass of <see cref="IWorkItem"/>.
        ///
        ///  NOTE: If the work item type provided in <paramref name="collection"/> does not match the type of <typeparamref name="T"/>
        ///  the empty list will be returned. For example, calling Create&lt;Bug&gt;() and passing it a set of Tasks will return the empty list.
        ///  </summary>
        ///  <typeparam name="T">The subclass of Issue that the work items should become</typeparam>
        ///  <param name="collection">The set of TFS WorkItems to convert</param>
        /// <returns>A cloned set of <see cref="IWorkItem"/>-subclassed items.</returns>
        IEnumerable<T> Create<T>([NotNull] IEnumerable<IWorkItem> collection) where T : IIdentifiable<int?>, new();

        IEnumerable<IIdentifiable<int?>> Create([NotNull] Type type, [NotNull] IEnumerable<IWorkItem> collection);

        /// <summary>
        /// Create a new, empty work item sub-class.
        /// </summary>
        /// <typeparam name="T">The type of sub-class to create</typeparam>
        /// <returns>A new work item of type T with the default value for all fields.</returns>
        [NotNull]
        T Default<T>() where T : new();

        [ItemNotNull]
        IEnumerable<IWorkItemMapperStrategy> MapperStrategies { get; }
    }
}
