using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Soap;

namespace Microsoft.Qwiq.Integration.Tests
{
    public static class IntegrationSettings
    {
        public static Func<IWorkItemStore> CreateSoapStore { get;  } = () =>
                                                                           {
                                                                               var options =
                                                                                       new AuthenticationOptions(
                                                                                                                 Uri,
                                                                                                                 AuthenticationTypes.Windows,
                                                                                                                 ClientType.Soap);
                                                                               return WorkItemStoreFactory.Instance.Create(options);
                                                                           };

        private static readonly Uri Uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

        public static Func<IWorkItemStore> CreateRestStore { get; } = () =>
                                                                                                                             {
                                                                                                                                 var options =
                                                                                                                                         new AuthenticationOptions(
                                                                                                                                                                   Uri,
                                                                                                                                                                   AuthenticationTypes.Windows,
                                                                                                                                                                   ClientType.Rest);
                                                                                                                                 return WorkItemStoreFactory.Instance.Create(options);
                                                                                                                             };
    }
}