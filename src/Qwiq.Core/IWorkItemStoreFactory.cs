using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(AuthenticationOptions options);

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        IWorkItemStore Create(Uri endpoint, TfsCredentials credentials);

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        IWorkItemStore Create(
            Uri endpoint,
            IEnumerable<TfsCredentials> credentials);
    }
}