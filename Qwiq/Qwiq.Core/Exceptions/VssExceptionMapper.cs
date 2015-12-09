using System;
using System.Linq;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.IE.Qwiq.Exceptions
{
    internal class VssExceptionMapper : IExceptionMapper
    {
        private readonly int[] _handledErrorCodes;
        private readonly Func<string, Exception, Exception> _newExceptionCreator;

        public VssExceptionMapper(int[] handledErrorCodes, Func<string, Exception, Exception> newExceptionCreator)
        {
            _handledErrorCodes = handledErrorCodes;
            _newExceptionCreator = newExceptionCreator;
        }

        public Exception Map(Exception ex)
        {
            var vssException = ex as VssException;
            return (vssException != null && _handledErrorCodes.Contains(vssException.ErrorCode)) ? _newExceptionCreator(vssException.Message, vssException) : null;
        }
    }
}
