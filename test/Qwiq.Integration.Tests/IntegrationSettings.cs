using System;

using Microsoft.Qwiq.Client.Rest;
using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq
{
    public static class IntegrationSettings
    {
        private static readonly Uri Uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

        public static Func<IWorkItemStore> CreateRestStore { get; } = () =>
                                                                          {
                                                                              var options = RestOptions;
                                                                              return WorkItemStoreFactory.Default.Create(options);
                                                                          };

        public static Func<IWorkItemStore> CreateSoapStore { get; } = () =>
                                                                          {
                                                                              var options = SoapOptions;
                                                                              return Client.Soap.WorkItemStoreFactory.Default.Create(options);
                                                                          };

        public static AuthenticationOptions RestOptions { get; } =
            new AuthenticationOptions(Uri, AuthenticationTypes.Windows, ClientType.Rest);

        public static AuthenticationOptions SoapOptions { get; } =
            new AuthenticationOptions(Uri, AuthenticationTypes.Windows, ClientType.Soap);

        public static Guid ProjectGuid = Guid.Parse("8d47e068-03c8-4cdc-aa9b-fc6929290322");

        public static string TenantId = "72F988BF-86F1-41AF-91AB-2D7CD011DB47";

        public static string[] Domains = new[] { "microsoft.com" };
    }
}