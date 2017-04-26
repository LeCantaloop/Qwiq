using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.WorkItemStore.WorkItem
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

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Core_identity_fields_contain_similar_information()
        {
            var exceptions = new List<Exception>();
            var identityFields = new[]
                                     {
                                         CoreFieldRefNames.AssignedTo, CoreFieldRefNames.AuthorizedAs,
                                         CoreFieldRefNames.ChangedBy, CoreFieldRefNames.CreatedBy
                                     };

            foreach (var field in identityFields)
            {
                try
                {
                    var restValue = RestResult.WorkItem[field]?.ToString();
                    var soapValue = SoapResult.WorkItem[field]?.ToString();

                    // If there is an identity field, drop the account name that REST returns to us
                    restValue = new IdentityFieldValue(restValue).DisplayName;
                    soapValue = new IdentityFieldValue(soapValue).DisplayName;

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
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Core_DateTime_fields_contain_similar_information()
        {
            var exceptions = new List<Exception>();
            var dateTimeFields = new[] { CoreFieldRefNames.ChangedDate, CoreFieldRefNames.CreatedDate, };

            foreach (var field in dateTimeFields)
            {
                try
                {
                    var restValue = (DateTime?)RestResult.WorkItem[field];
                    var soapValue = (DateTime)SoapResult.WorkItem[field];

                    restValue.GetValueOrDefault().ShouldEqual(soapValue.ToUniversalTime(), field);
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
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void CoreFields_are_equal()
        {
            var exceptions = new List<Exception>();


            var fieldsWithKnownDifferences = new[]
                                                 {
                                                     CoreFieldRefNames.AttachedFileCount,
                                                     CoreFieldRefNames.AuthorizedDate,
                                                     CoreFieldRefNames.BoardColumn,
                                                     CoreFieldRefNames.BoardLane,
                                                     CoreFieldRefNames.ChangedDate,
                                                     CoreFieldRefNames.CreatedDate,
                                                     CoreFieldRefNames.ExternalLinkCount,
                                                     CoreFieldRefNames.HyperlinkCount,
                                                     CoreFieldRefNames.RelatedLinkCount,
                                                     CoreFieldRefNames.AssignedTo, CoreFieldRefNames.AuthorizedAs,
                                                     CoreFieldRefNames.ChangedBy, CoreFieldRefNames.CreatedBy
                                                 };

            foreach (var field in CoreFieldRefNames.All.Except(fieldsWithKnownDifferences))
            {
                try
                {

                    var restValue = RestResult.WorkItem[field]?.ToString();
                    var soapValue = SoapResult.WorkItem[field]?.ToString();


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
