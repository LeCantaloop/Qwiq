using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq
{
    public class ItemAlreadyUpdatedOnServerException : InvalidOperationException
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
