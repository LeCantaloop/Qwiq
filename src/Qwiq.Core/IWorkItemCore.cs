namespace Qwiq
{
    public interface IWorkItemCore : IWorkItemReference
    {
        int? Rev { get; }

        /// <summary>
        ///     Gets or sets the value of a field in this work item that is specified by
        ///     the field name.
        /// </summary>
        /// <param name="name">
        ///     The string that is passed in name could be either the field name or a reference name.
        /// </param>
        /// <returns>The object that is contained in this field.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     The name parameter is null.
        /// </exception>
        object this[string name] { get; set; }

        // Relations

        // Links
    }
}