using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
    /// <summary>
    /// A compatibility shim for IdentityTypeMapper which was removed from newer versions
    /// of Microsoft.VisualStudio.Services.Client (v19+).
    ///
    /// <para><strong>WHY THIS EXISTS:</strong></para>
    /// <para>
    /// The original <c>Microsoft.VisualStudio.Services.Identity.IdentityTypeMapper</c> class was
    /// part of the Azure DevOps/TFS client libraries but was removed in version 19.0 as part of
    /// internal refactoring. However, the QWIQ library depends on identity type resolution for
    /// mapping between identity type names (like "Microsoft.TeamFoundation.Identity") and their
    /// corresponding byte IDs used in the TFS/Azure DevOps wire protocol.
    /// </para>
    ///
    /// <para><strong>WHAT IT DOES:</strong></para>
    /// <para>
    /// This shim provides a bidirectional mapping between identity type names and byte IDs:
    /// <list type="bullet">
    ///   <item><see cref="GetTypeNameFromId"/> - Converts a byte ID to its identity type name</item>
    ///   <item><see cref="GetTypeIdFromName"/> - Converts an identity type name to its byte ID</item>
    /// </list>
    /// Unknown types are dynamically registered to support extensibility.
    /// </para>
    ///
    /// <para><strong>WHEN IT CAN BE REMOVED:</strong></para>
    /// <para>
    /// This shim can be removed when:
    /// <list type="number">
    ///   <item>QWIQ fully migrates to modern Azure DevOps REST APIs that don't require identity type mapping</item>
    ///   <item>The upstream Microsoft.VisualStudio.Services.Client library re-exposes equivalent functionality</item>
    ///   <item>All SOAP-based TFS client dependencies are removed from the project</item>
    /// </list>
    /// </para>
    ///
    /// <para><strong>THREAD SAFETY:</strong></para>
    /// <para>
    /// This class is thread-safe. The singleton instance is lazily initialized using <see cref="Lazy{T}"/>,
    /// and all dictionary mutations are protected by a lock to prevent race conditions when
    /// registering unknown identity types concurrently.
    /// </para>
    /// </summary>
    internal sealed class IdentityTypeMapper
    {
        private static readonly Lazy<IdentityTypeMapper> LazyInstance =
            new Lazy<IdentityTypeMapper>(() => new IdentityTypeMapper());

        internal static IdentityTypeMapper Instance => LazyInstance.Value;
        /// <summary>
        /// Lock object for thread-safe dictionary mutations when registering unknown types.
        /// </summary>
        private readonly object _lock = new object();

        private readonly Dictionary<byte, string> _idToName = new Dictionary<byte, string>
        {
            { 0, "System.Security.Principal.WindowsIdentity" },
            { 1, "Microsoft.TeamFoundation.Identity" },
            { 2, "Microsoft.TeamFoundation.ServiceIdentity" },
            { 3, "Microsoft.TeamFoundation.UnauthenticatedIdentity" },
            { 4, "Microsoft.IdentityModel.Claims.ClaimsIdentity" },
            { 5, "Microsoft.TeamFoundation.Framework.Server.TeamFoundationApplicationGroup" },
            { 6, "Microsoft.TeamFoundation.GroupIdentity" },
            { 7, "Microsoft.TeamFoundation.BindPendingIdentity" },
            { 8, "Microsoft.TeamFoundation.ImportedIdentity" },
            { 9, "Microsoft.TeamFoundation.AggregateIdentity" },
            { 10, "Microsoft.TeamFoundation.ServerIdentity" },
            { 11, "Microsoft.TeamFoundation.CertificateIdentity" },
            { 12, "Microsoft.TeamFoundation.SystemIdentity" },
            { 13, "Microsoft.TeamFoundation.ServicePrincipal" },
            { 14, "Microsoft.VisualStudio.Services.Identity.AadUser" },
            { 15, "Microsoft.VisualStudio.Services.Identity.MsaUser" },
            { 16, "Microsoft.VisualStudio.Services.Identity.VssUser" },
        };

        private readonly Dictionary<string, byte> _nameToId = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

        private IdentityTypeMapper()
        {
            foreach (var kvp in _idToName)
            {
                _nameToId[kvp.Value] = kvp.Key;
            }
        }

        /// <summary>
        /// Gets the identity type name for a given type ID.
        /// </summary>
        /// <param name="typeId">The byte identifier of the identity type.</param>
        /// <returns>The type name if found; otherwise, a string in the format "UnknownIdentityType_{id}".</returns>
        internal string GetTypeNameFromId(byte typeId)
        {
            lock (_lock)
            {
                if (_idToName.TryGetValue(typeId, out var name))
                {
                    return name;
                }
            }
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "UnknownIdentityType_{0}", typeId);
        }

        /// <summary>
        /// Gets the byte identifier for a given identity type name.
        /// If the type name is not already registered, it will be dynamically registered with a new ID.
        /// </summary>
        /// <param name="typeName">The identity type name to look up.</param>
        /// <returns>The byte identifier for the type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="typeName"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the maximum number of identity types (255) has been exceeded.</exception>
        internal byte GetTypeIdFromName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            // Fast path: check without lock first for registered types (most common case)
            lock (_lock)
            {
                if (_nameToId.TryGetValue(typeName, out var id))
                {
                    return id;
                }

                // Register the unknown type
                byte nextId = (byte)(_idToName.Count);
                while (_idToName.ContainsKey(nextId) && nextId < byte.MaxValue)
                {
                    nextId++;
                }

                if (nextId == byte.MaxValue && _idToName.ContainsKey(nextId))
                {
                    throw new InvalidOperationException("Maximum number of identity types exceeded.");
                }

                _idToName[nextId] = typeName;
                _nameToId[typeName] = nextId;

                return nextId;
            }
        }
    }
}