using System;

namespace Microsoft.Qwiq
{
    [Serializable]
    public class ServerRejectedChangesException : InvalidOperationException
    {
        internal ServerRejectedChangesException(Exception ex) : base(ex.Message, ex)
        {
        }
    }
}
