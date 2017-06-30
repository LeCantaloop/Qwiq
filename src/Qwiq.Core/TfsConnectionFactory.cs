using Microsoft.Qwiq.Credentials;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Qwiq
{
    public abstract class TfsConnectionFactory<TConnection> : ITfsConnectionFactory
        where TConnection : ITeamProjectCollection
    {
        public virtual ITeamProjectCollection Create(AuthenticationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var credentials = options.Credentials;

            foreach (var credential in credentials)
            {
                try
                {
                    var tfs = ConnectToTfsCollection(options.Uri, credential);
                    options.Notifications?.AuthenticationSuccess(new AuthenticationSuccessNotification(credential, tfs));
                    return tfs;
                }
                catch (Exception e)
                {
                    options.Notifications?.AuthenticationFailed(new AuthenticationFailedNotification(credential, e));
                }
            }

            var nocreds = new AccessDeniedException("Invalid credentials");
            options.Notifications?.AuthenticationFailed(new AuthenticationFailedNotification(null, nocreds));
            throw nocreds;
        }

        protected abstract TConnection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials);
    }
}