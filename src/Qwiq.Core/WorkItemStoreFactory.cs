using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;

using TfsSoap = Microsoft.Qwiq.Proxies.Soap;
using TfsRest = Microsoft.Qwiq.Proxies.Rest;

namespace Microsoft.Qwiq
{
    public enum ClientType : short
    {
        Default = 0,

        Soap = 0,

        Rest = 1
    }

    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(AuthenticationOptions options);

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        IWorkItemStore Create(Uri endpoint, TfsCredentials credentials, ClientType type = ClientType.Default);

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        IWorkItemStore Create(
            Uri endpoint,
            IEnumerable<TfsCredentials> credentials,
            ClientType type = ClientType.Default);
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
                    var tfsProxy =
                        ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                            new TfsTeamProjectCollectionProxy(tfsNative));

                    options.Notifications.AuthenticationSuccess(
                        new AuthenticationSuccessNotification(credential, tfsProxy));

                    IWorkItemStore wis;
                    switch (options.ClientType)
                    {
                        case ClientType.Rest:
                            wis = CreateRestWorkItemStore(tfsNative);
                            break;

                        case ClientType.Soap:
                            wis = CreateSoapWorkItemStore(tfsNative);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(options.ClientType));
                    }

                    return ExceptionHandlingDynamicProxyFactory.Create(wis);
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
        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials, ClientType type = ClientType.Default)
        {
            return Create(endpoint, new[] { credentials }, type);
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See Create(AuthenticationOptions) instead.",
            false)]
        public IWorkItemStore Create(
            Uri endpoint,
            IEnumerable<TfsCredentials> credentials,
            ClientType type = ClientType.Default)
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

        private static IWorkItemStore CreateRestWorkItemStore(TfsTeamProjectCollection tfs)
        {
            return new TfsRest.WorkItemStoreProxy(tfs, TfsRest.QueryFactory.GetInstance);
        }

        private static IWorkItemStore CreateSoapWorkItemStore(TfsTeamProjectCollection tfs)
        {
            return new TfsSoap.WorkItemStoreProxy(tfs, TfsSoap.QueryFactory.GetInstance);
        }

        private static IEnumerable<TfsCredentials> EnumerateCredentials(AuthenticationOptions options)
        {
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.OAuth)) yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.PersonalAccessToken))
                yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.Basic)) yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.Windows))
                yield return credential;
            foreach (var credential in EnumerateCredentials(options, AuthenticationType.Anonymous))
                yield return credential;
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