using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.IE.Qwiq.Proxies;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class ItemAlreadyUpdatedOnServerException : Exception
    {
        private readonly Tfs.ItemAlreadyUpdatedOnServerException _exception;

        internal ItemAlreadyUpdatedOnServerException(Tfs.ItemAlreadyUpdatedOnServerException exception) : base(exception.Message, exception)
        {
            _exception = exception;
        }

        public IEnumerable<IFieldConflict> FieldConflicts
        {
            get { return _exception.FieldConflicts.Select(item => ExceptionHandlingDynamicProxyFactory.Create<IFieldConflict>(new FieldConflictProxy(item))); }
        }
    }
}