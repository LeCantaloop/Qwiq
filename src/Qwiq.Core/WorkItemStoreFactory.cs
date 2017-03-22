using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;

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
        IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials);
    }

    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        private static readonly Lazy<WorkItemStoreFactory> Instance =
            new Lazy<WorkItemStoreFactory>(() => new WorkItemStoreFactory());

        private WorkItemStoreFactory()
        {
        }

        public IWorkItemStore Create(AuthenticationOptions options)
        {
            var credentials = EnumerateCredentials(options);

            foreach (var credential in credentials)
                try
                {
                    var tfsNative = ConnectToTfsCollection(options.Uri, credential.Credentials);
                    var tfsProxy = ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(new TfsTeamProjectCollectionProxy(tfsNative));

                    options.Notifications.AuthenticationSuccess(new AuthenticationSuccessNotification(credential, tfsProxy));

                    var proxy = new WorkItemStoreProxy(()=>tfsProxy, QueryFactory.GetInstance);
                    return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemStore>(proxy);
                }
                catch (TeamFoundationServerUnauthorizedException e)
                {
                    options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(credential, e));
                }

            var nocreds = new AccessDeniedException("Invalid credentials");
            options.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(null, nocreds));
            throw nocreds;
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials)
        {
            return Create(endpoint, new[] { credentials });
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials)
        {
            var options =
                new AuthenticationOptions(
                    endpoint,
                    AuthenticationType.Windows) { CreateCredentials = t => credentials };

            return Create(options);
        }

        public static IWorkItemStoreFactory GetInstance()
        {
            return Instance.Value;
        }

        private static TfsTeamProjectCollection ConnectToTfsCollection(Uri endpoint, VssCredentials credentials)
        {
            var tfsServer = new TfsTeamProjectCollection(endpoint, credentials);
            tfsServer.EnsureAuthenticated();
            return tfsServer;
        }

        private static IEnumerable<TfsCredentials> EnumerateCredentials(AuthenticationOptions options)
        {
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.OAuth)) yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.PersonalAccessToken)) yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.Basic)) yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.Windows)) yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.Anonymous)) yield return credential;
        }

        private static IEnumerable<TfsCredentials> EnumerateCredentials(
            AuthenticationOptions authenticationOptions,
            AuthenticationType authenticationType)
        {
            if (!authenticationOptions.AuthenticationType.HasFlag(authenticationType)) yield break;

            var credentials = Enumerable.Empty<TfsCredentials>();

            try
            {
                credentials = authenticationOptions.CreateCredentials(authenticationType);
            }
            catch (Exception e)
            {
                authenticationOptions.Notifications.AuthenticationFailed(
                    new AuthenticationFailedNotification(null) { Exception = e });
                throw;
            }

            foreach (var credential in credentials) yield return credential;
        }
    }
}