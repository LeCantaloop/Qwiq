using System;

namespace Microsoft.Qwiq
{
    public class ServerRejectedChangesException : InvalidOperationException
    {
        internal ServerRejectedChangesException(Exception ex) : base(ex.Message, ex)
        {
        }
    }
}
