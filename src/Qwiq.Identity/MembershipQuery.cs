namespace Qwiq.Identity
{
    /// <summary>
    /// Indicates the method to populate <see cref="ITeamFoundationIdentity"/>
    /// </summary>
    public enum MembershipQuery
    {
        /// <summary>
        /// Query will not return any membership data
        /// </summary>
        None = 0,
        /// <summary>
        ///     Query will return only direct membership data
        /// </summary>
        Direct = 1,
        /// <summary>
        ///     Query will return expanded membership data
        /// </summary>
        Expanded = 2,
        /// <summary>
        ///     Query will return expanded up membership data (parents only)
        /// </summary>
        ExpandedUp = 3,
        /// <summary>
        ///     Query will return expanded down membership data (children only)
        /// </summary>
        ExpandedDown = 4
    }
}