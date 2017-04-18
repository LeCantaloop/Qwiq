using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    [TestClass]
    public class Given_a_WorkItem_from_each_WorkItemStore_implementation : IntegrationContextSpecificationSpecification
    {
        private const int Id = 10726528;

        public override void When()
        {
            SoapResult.WorkItem = TimedAction(() => SoapResult.WorkItemStore.Query(Id), "SOAP", "Query By Id");
            RestResult.WorkItem = TimedAction(() => RestResult.WorkItemStore.Query(Id), "REST", "Query By Id");
        }
    }

    public abstract class IntegrationContextSpecificationSpecification : WorkItemStoreComparisonContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void AreaPath_is_equal()
        {
            RestResult.WorkItem.AreaPath.ShouldEqual(SoapResult.WorkItem.AreaPath);

            RestResult.WorkItem[CoreFieldRefNames.AreaPath].ShouldEqual(RestResult.WorkItem.AreaPath);
            RestResult.WorkItem.Fields[CoreFieldRefNames.AreaPath].Value.ShouldEqual(RestResult.WorkItem.AreaPath);

            SoapResult.WorkItem[CoreFieldRefNames.AreaPath].ShouldEqual(SoapResult.WorkItem.AreaPath);
        }

        public override void Cleanup()
        {
            SoapResult?.Dispose();
            RestResult?.Dispose();

            base.Cleanup();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void CoreFields_are_equal()
        {
            var exceptions = new List<Exception>();
            var identityFields = new[]
                                     {
                                         CoreFieldRefNames.AssignedTo, CoreFieldRefNames.AuthorizedAs,
                                         CoreFieldRefNames.ChangedBy, CoreFieldRefNames.CreatedBy
                                     };
            foreach (var field in CoreFieldRefNames.All)
            {
                try
                {

                    var restValue = RestResult.WorkItem[field]?.ToString();
                    var soapValue = SoapResult.WorkItem[field]?.ToString();

                    // If there is an identity field, drop the account name that REST returns to us
                    if (identityFields.Contains(field))
                    {
                        restValue = new IdentityFieldValue(restValue).DisplayName;
                        soapValue = new IdentityFieldValue(soapValue).DisplayName;
                    }



                    restValue.ShouldEqual(soapValue, field);

                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions.EachToUsefulString(), exceptions);
            }
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void CreatedDate_is_equal()
        {
            RestResult.WorkItem.CreatedDate.ShouldEqual(SoapResult.WorkItem.CreatedDate.ToUniversalTime());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void RelatedLinkCount_is_equal()
        {
            RestResult.WorkItem.RelatedLinkCount.ShouldEqual(SoapResult.WorkItem.RelatedLinkCount);
        }
    }
}
