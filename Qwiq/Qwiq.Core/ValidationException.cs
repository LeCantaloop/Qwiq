using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class ValidationException : Exception
    {
        internal ValidationException(Tfs.ValidationException ex) : base(ex.Message)
        {
        }
    }
}
