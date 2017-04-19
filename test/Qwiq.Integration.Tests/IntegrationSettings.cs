using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Soap;

namespace Microsoft.Qwiq.Integration.Tests
{
    public static class IntegrationSettings
    {
        public static readonly Func<IWorkItemStore> CreateRestStore = () =>
                                                                          {
                                                                              var options =
                                                                                      new AuthenticationOptions(
                                                                                                                Uri,
                                                                                                                AuthenticationTypes.Windows,
                                                                                                                ClientType.Rest);
                                                                              return WorkItemStoreFactory.Instance.Create(options);
                                                                          };

        public static readonly Func<IWorkItemStore> CreateSoapStore = () =>
                                                                          {
                                                                              var options =
                                                                                      new AuthenticationOptions(
                                                                                                                Uri,
                                                                                                                AuthenticationTypes.Windows,
                                                                                                                ClientType.Soap);
                                                                              return WorkItemStoreFactory.Instance.Create(options);
                                                                          };

        private static readonly Uri Uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");
    }
}