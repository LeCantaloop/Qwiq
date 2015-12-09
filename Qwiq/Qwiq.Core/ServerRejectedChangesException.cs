using System;

namespace Microsoft.IE.Qwiq
{
    public class ServerRejectedChangesException : Exception
    {
        internal ServerRejectedChangesException(Exception ex) : base(ex.Message, ex)
        {
        }
    }
}