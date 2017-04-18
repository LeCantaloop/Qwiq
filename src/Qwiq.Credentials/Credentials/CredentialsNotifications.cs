using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Qwiq.Credentials
{
    /// <summary>
    /// Specifies events which the <see cref="IWorkItemStoreFactory"/> invokes to enable developer control over the authentication process.
    /// </summary>
    public class CredentialsNotifications : ICredentialsNotifications
    {
        public CredentialsNotifications()
        {
            AuthenticationFailed = n =>
                {
                    var credential = n.Credentials;
                    var e = n.Exception;

                    if (credential != null)
                    {

                        Trace.TraceWarning(
                            "TFS connection attempt failed with {0}/{1}.\n Exception: {2}",
                            credential.Windows?.GetType(),
                            credential.Federated?.GetType(),
                            e?.ToString() ?? "<none>");
                    }
                    else
                    {
                        Trace.TraceError("TFS connection attempt failed. {0}", e?.ToString() ?? "No additional information.");
                    }

                    return Task.CompletedTask;
                };

            AuthenticationSuccess = n =>
                {
                    var credential = n.Credentials;

                    Trace.TraceInformation(
                        $"Connected to {n.TeamProjectCollection.Uri}: {credential}");

                    Trace.TraceInformation(
                        "TFS connection attempt success with {0}/{1}.",
                        credential.Windows.GetType(),
                        credential.Federated.GetType());

                    return Task.CompletedTask;
                };
        }
        public Func<IAuthenticationFailedNotification, Task> AuthenticationFailed { get; set; }
        public Func<IAuthenticationSuccessNotification, Task> AuthenticationSuccess { get; set; }
    }
}