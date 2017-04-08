using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class IntegrationContextSpecificationSpecification : WorkItemStoreComparisonContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void AreaPath_is_equal()
        {
            RestResult.WorkItem.AreaPath.ShouldEqual(SoapResult.WorkItem.AreaPath);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void AssignedTo_is_equal()
        {
            RestResult.WorkItem.AssignedTo.ShouldEqual(SoapResult.WorkItem.AssignedTo);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void ChangedBy_is_equal()
        {
            RestResult.WorkItem.ChangedBy.ShouldEqual(SoapResult.WorkItem.ChangedBy);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void ChangedDate_is_equal()
        {
            RestResult.WorkItem.ChangedDate.ShouldEqual(SoapResult.WorkItem.ChangedDate.ToUniversalTime());
        }

        public override void Cleanup()
        {
            SoapResult?.Dispose();
            RestResult?.Dispose();

            base.Cleanup();
        }

        [TestMethod]
        [TestCategory("localOnly")]
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
        public void CreatedBy_is_equal()
        {
            RestResult.WorkItem.CreatedBy.ShouldEqual(SoapResult.WorkItem.CreatedBy);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Rev_is_equal()
        {
            RestResult.WorkItem.Rev.ShouldEqual(SoapResult.WorkItem.Rev);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void CreatedDate_is_equal()
        {
            RestResult.WorkItem.CreatedDate.ShouldEqual(SoapResult.WorkItem.CreatedDate.ToUniversalTime());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Description_is_equal()
        {
            RestResult.WorkItem.Description.ShouldEqual(SoapResult.WorkItem.Description);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void History_is_equal()
        {
            RestResult.WorkItem.History.ShouldEqual(SoapResult.WorkItem.History);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Id_is_equal()
        {
            RestResult.WorkItem.Id.ShouldEqual(SoapResult.WorkItem.Id);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void IterationPath_is_equal()
        {
            RestResult.WorkItem.IterationPath.ShouldEqual(SoapResult.WorkItem.IterationPath);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void REST_WorkItem_is_returned()
        {
            RestResult.WorkItem.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void SOAP_WorkItem_is_returned()
        {
            SoapResult.WorkItem.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void State_is_equal()
        {
            RestResult.WorkItem.State.ShouldEqual(SoapResult.WorkItem.State);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Tags_is_equal()
        {
            RestResult.WorkItem.Tags.ShouldEqual(SoapResult.WorkItem.Tags);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Title_is_equal()
        {
            RestResult.WorkItem.Title.ShouldEqual(SoapResult.WorkItem.Title);
        }


    }
}
