using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// Represents the work item store resource.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IWorkItemStore : IDisposable
    {
        /// <summary>
        /// Indicates the communication type used for the work item store.
        /// </summary>
        ClientType ClientType { get; }

        /// <summary>Gets the credentials for this project collection.</summary>
        /// <value>The authorized credentials.</value>
        TfsCredentials AuthorizedCredentials { get; }

        /// <summary>
        /// Returns the collection of all known <see cref="IFieldDefinition"/>s associated with this instance.
        /// </summary>
        /// <value>The field definitions.</value>
        IFieldDefinitionCollection FieldDefinitions { get; }

        /// <summary>
        /// Gets a collection of <see cref="IProject"/> associated with this instance.
        /// </summary>
        /// <value>The projects.</value>
        IProjectCollection Projects { get; }

        /// <summary>
        /// Gets the team project collection.
        /// </summary>
        /// <value>The team project collection.</value>
        ITfsTeamProjectCollection TeamProjectCollection { get; }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        /// <value>The time zone.</value>
        TimeZone TimeZone { get; }

        /// <summary>
        /// Gets the name of the user account.
        /// </summary>
        /// <value>The name of the user account.</value>
        string UserAccountName { get; }

        /// <summary>
        /// Gets the display name of the user.
        /// </summary>
        /// <value>The display name of the user.</value>
        string UserDisplayName { get; }

        /// <summary>
        /// Gets the user sid.
        /// </summary>
        /// <value>The user sid.</value>
        string UserSid { get; }

        /// <summary>
        /// Gets the work item link types associated with this instance.
        /// </summary>
        /// <value>The work item link types.</value>
        WorkItemLinkTypeCollection WorkItemLinkTypes { get; }

        /// <summary>
        /// Runs a query using the WIQL string passed in and returns a collection of <see cref="IWorkItem"/>.
        /// </summary>
        /// <param name="wiql">The WIQL query string.</param>
        /// <param name="dayPrecision">
        ///     <c>True</c> to ignore time values so that DateTime objects are treated as dates; otherwise,
        ///     <c>false</c>.
        /// </param>
        /// <returns>IEnumerable&lt;IWorkItem&gt;.</returns>
        IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false);

        /// <summary>
        /// Queries the specified work item IDs.
        /// </summary>
        /// <param name="ids">A collection of work item IDs.</param>
        /// <param name="asOf">Optional: The date of the desired work item state.</param>
        /// <returns>IEnumerable&lt;IWorkItem&gt;.</returns>
        IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null);

        /// <summary>
        /// Queries the specified work item ID.
        /// </summary>
        /// <param name="id">The work item ID.</param>
        /// <param name="asOf">Optional: The desired date of the work item state.</param>
        /// <returns><see cref="IWorkItem"/></returns>
        IWorkItem Query(int id, DateTime? asOf = null);

        /// <summary>
        /// Queries the links.
        /// </summary>
        /// <param name="wiql">The WIQL query.</param>
        /// <param name="dayPrecision">if set to <c>true</c> [day precision].</param>
        /// <returns>IEnumerable&lt;IWorkItemLinkInfo&gt;.</returns>
        IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false);
    }
}