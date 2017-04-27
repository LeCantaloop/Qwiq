using Microsoft.Qwiq.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public abstract class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        /// <inheritdoc />
        public abstract IWorkItemStore Create(AuthenticationOptions options);

        [Obsolete("This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.", false)]
        public virtual IWorkItemStore Create(Uri endpoint, TfsCredentials credentials)
        {
            return Create(endpoint, new[] { credentials });
        }

        [Obsolete("This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.", false)]
        public virtual IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials)
        {
            var options = new AuthenticationOptions(endpoint, AuthenticationTypes.Windows, types => credentials.Select(s => s.Credentials));
            return Create(options);
        }
    }
}