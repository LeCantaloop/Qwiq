using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq
{
    public class ValidationException : InvalidOperationException
    {
        internal ValidationException(Tfs.ValidationException ex) : base(ex.Message, ex)
        {
        }
    }
}

