using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Qwiq.Credentials
{
    /// <summary>
    ///     Specifies events which the <see cref="IWorkItemStoreFactory" /> invokes to enable developer control over the
    ///     authentication process.
    /// </summary>
    public class CredentialsNotifications
    {
        public CredentialsNotifications()
            : this(null, null)
        {
        }

        public CredentialsNotifications(
            Func<AuthenticationSuccessNotification, Task> success,
            Func<AuthenticationFailedNotification, Task> failed)
        {
            AuthenticationSuccess = success ?? SuccessAsync;
            AuthenticationFailed = failed ?? FailedAsync;
        }

        public Func<AuthenticationFailedNotification, Task> AuthenticationFailed { get; }

        public Func<AuthenticationSuccessNotification, Task> AuthenticationSuccess { get; }

        private static Task FailedAsync(AuthenticationFailedNotification n)
        {
            var credential = n.Credentials;
            var e = n.Exception;

            if (credential != null)
                Trace.TraceWarning(
                                   "TFS connection attempt failed with {0}/{1}.\n Exception: {2}",
                                   credential.Windows?.GetType(),
                                   credential.Federated?.GetType(),
                                   e?.ToString() ?? "<none>");
            else Trace.TraceError("TFS connection attempt failed. {0}", e?.ToString() ?? "No additional information.");

            return Task.CompletedTask;
        }

        private static Task SuccessAsync(AuthenticationSuccessNotification n)
        {
            var credential = n.Credentials;

            FormattableString s = $"Connected to {n.TeamProjectCollection.Uri}: {n.TeamProjectCollection.AuthorizedIdentity.UniqueName}";

            Trace.TraceInformation(s.ToString(CultureInfo.InvariantCulture));

            Trace.TraceInformation(
                                   "TFS connection attempt success with {0}/{1}.",
                                   credential.Windows.GetType(),
                                   credential.Federated.GetType());

            return Task.CompletedTask;
        }
    }
}