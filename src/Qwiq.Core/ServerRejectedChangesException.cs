using System;

namespace Qwiq
{
    [Serializable]
    public class ServerRejectedChangesException : InvalidOperationException
    {
        internal ServerRejectedChangesException(Exception ex) : base(ex.Message, ex)
        {
        }
    }
}
