using System;

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    public class ValidationException : InvalidOperationException
    {
        internal ValidationException(string message)
            : base(message)
        {
        }

        internal ValidationException(SyntaxException ex)
            : base(ex.Details, ex)
        {
        }

        internal ValidationException(Tfs.ValidationException ex) : base(ex.Message, ex)
        {
        }
    }
}

