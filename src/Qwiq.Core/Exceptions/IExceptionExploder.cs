using System;
using System.Collections.ObjectModel;


namespace Qwiq.Exceptions
{
    /// <summary>
    /// Provides a mechanism to expand an exception into a collection of exceptions.
    /// </summary>
    public interface IExceptionExploder
    {
        /// <summary>
        /// Expands the given exception into a collection of exceptions.
        /// </summary>
        /// <param name="exception">The exception to expand.</param>
        /// <returns>
        /// A read-only collection of exceptions extracted from the given exception.
        /// This method never returns null; if no expansion is possible, returns a collection
        /// containing only the original exception.
        /// </returns>
        ReadOnlyCollection<Exception> Explode(Exception exception);
    }
}

