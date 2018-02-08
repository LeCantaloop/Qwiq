using System;
using System.Collections.Generic;

namespace Qwiq
{
    /// <summary>
    /// Initializes a new instance of <see cref="IQuery"/>.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        ///     Creates an instance of <see cref="IQuery" /> that is described in <paramref name="wiql" />.
        /// </summary>
        /// <param name="wiql">The work item query string to execute.</param>
        /// <param name="dayPrecision">
        ///     <c>True</c> to ignore time values so that DateTime objects are treated as dates; otherwise,
        ///     <c>false</c>.
        /// </param>
        /// <returns>IQuery.</returns>
        IQuery Create(string wiql, bool dayPrecision = false);

        /// <summary>
        ///     Create an instance of <see cref="IQuery" /> with a set of fields that is referred to in <paramref name="wiql" />
        ///     that are specified by an ID number (<paramref name="ids" />).
        /// </summary>
        /// <param name="ids">A collection of work item IDs.</param>
        /// <param name="wiql">WIQL definition of fields to return.</param>
        /// <returns>IQuery.</returns>
        IQuery Create(IEnumerable<int> ids, string wiql);

        /// <summary>
        ///     Create an instance of <see cref="IQuery" /> for work items specified by ID number (<paramref name="ids" />) at an
        ///     optional previous date.
        /// </summary>
        /// <param name="ids">A collection of work item IDs.</param>
        /// <param name="asOf">Optional: The date of the desired work item state.</param>
        /// <returns><see cref="IQuery"/></returns>
        IQuery Create(IEnumerable<int> ids, DateTime? asOf = null);
    }
}