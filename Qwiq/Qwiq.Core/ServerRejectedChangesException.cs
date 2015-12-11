using System;

namespace Microsoft.IE.Qwiq
{
    public class ServerRejectedChangesException : InvalidOperationException
    {
        internal ServerRejectedChangesException(Exception ex) : base(ex.Message, ex)
        {
        }
    }
}