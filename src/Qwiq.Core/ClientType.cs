namespace Microsoft.Qwiq
{
    /// <summary>
    /// Enum ClientType
    /// </summary>
    public enum ClientType : short
    {
        /// <summary>
        /// No remote communications are made.
        /// </summary>
        None = 0,

        /// <summary>
        /// The default client type: <see cref="Soap"/>.
        /// </summary>
        Default = Soap,

        /// <summary>
        /// The SOAP client integrates with Microsoft Team Foundation Server 2012 and later and Visual Studio Team Services via SOAP APIs.
        /// </summary>
        Soap = 1,

        /// <summary>
        /// The REST client integrates with Team Foundation Server 2015 and later and Visual Studio Team Services via public REST APIs.
        /// </summary>
        Rest = 2
    }
}