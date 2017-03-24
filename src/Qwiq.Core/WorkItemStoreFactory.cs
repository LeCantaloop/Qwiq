using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using TfsSoap = Microsoft.Qwiq.Proxies.Soap;
using TfsRest = Microsoft.Qwiq.Proxies.Rest;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Client;

namespace Microsoft.Qwiq
{
    public interface IWorkItemStoreFactory
    {
        IWorkItemStore Create(Uri endpoint, TfsCredentials credentials, ClientType type = ClientType.Default);

        IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials, ClientType type = ClientType.Default);
    }

    public enum ClientType : short
    {
        Default = 0,
        Soap = 0,
        Rest = 1
    }

    public class WorkItemStoreFactory : IWorkItemStoreFactory
    {
        private static readonly Lazy<WorkItemStoreFactory> Instance =
            new Lazy<WorkItemStoreFactory>(() => new WorkItemStoreFactory());

        private WorkItemStoreFactory()
        {
        }

        public static IWorkItemStoreFactory GetInstance()
        {
            return Instance.Value;
        }

        public IWorkItemStore Create(Uri endpoint, TfsCredentials credentials, ClientType type = ClientType.Default)
        {
            return Create(endpoint, new[] { credentials }, type);
        }

        public IWorkItemStore Create(Uri endpoint, IEnumerable<TfsCredentials> credentials, ClientType type = ClientType.Default)
        {
            foreach (var credential in credentials)
            {
                try
                {
                    var tfsNative = ConnectToTfsCollection(endpoint, credential.Credentials);

                    System.Diagnostics.Trace.TraceInformation(
                        "TFS connection attempt success with {0}/{1}.",
                        credential.Credentials.Windows.GetType(),
                        credential.Credentials.Federated.GetType());

                    IWorkItemStore wis;
                    switch (type)
                    {
                        case ClientType.Rest:
                            wis = CreateRestWorkItemStore(tfsNative);
                            break;
                        case ClientType.Soap:
                            wis = CreateSoapWorkItemStore(tfsNative);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type));
                    }

                    return ExceptionHandlingDynamicProxyFactory.Create(wis);
                }
                catch (TeamFoundationServerUnauthorizedException e)
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "TFS connection attempt failed with {0}/{1}.\n Exception: {2}",
                        credential.Credentials.Windows.GetType(),
                        credential.Credentials.Federated.GetType(),
                        e);
                }
            }

            System.Diagnostics.Trace.TraceError("All TFS connection attempts failed.");
            throw new AccessDeniedException("Invalid credentials");
        }

        private static IWorkItemStore CreateRestWorkItemStore(TfsTeamProjectCollection tfs)
        {
            return new TfsRest.WorkItemStoreProxy(tfs, TfsRest.QueryFactory.GetInstance);
        }

        private static IWorkItemStore CreateSoapWorkItemStore(TfsTeamProjectCollection tfs)
        {
            return new TfsSoap.WorkItemStoreProxy(tfs, TfsSoap.QueryFactory.GetInstance);
        }

        private static TfsTeamProjectCollection ConnectToTfsCollection(
            Uri endpoint,
            TfsClientCredentials credentials)
        {
            var tfsServer = new TfsTeamProjectCollection(endpoint, credentials);
            tfsServer.EnsureAuthenticated();

#if DEBUG
            // The base class TfsConnection integrates various information about the connect with TFS (as well as ways to control that connection)
            System.Diagnostics.Debug.Print($"Connected to {endpoint}");
            System.Diagnostics.Debug.Print($"Authenticated: {tfsServer.AuthorizedIdentity.Descriptor.Identifier}");
#endif
            return tfsServer;
        }
    }
}