using System.Linq;
using System.Linq.Expressions;

using Microsoft.Qwiq.Identity;
using Microsoft.Qwiq.Identity.Soap;
using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Integration.Tests
{
    public abstract class SoapIdentityMapperContextSpecification<T> : TimedContextSpecification
    {
        protected IdentityAliasValueConverter Instance { get; set; }
        protected T Input { get; set; }
        protected T ActualOutput { get; set; }
        protected T ExpectedOutput { get; set; }

        /// <inheritdoc />
        public override void Given()
        {
            base.Given();

            var wis = TimedAction(() => IntegrationSettings.CreateSoapStore(), "SOAP", "WIS Create");
            var soapIms = TimedAction(() => wis.GetIdentityManagementService(), "SOAP", "IMS Create");
            Instance = new IdentityAliasValueConverter(soapIms, "72F988BF-86F1-41AF-91AB-2D7CD011DB47", "microsoft.com");
        }

        public override void When()
        {
            ActualOutput = TimedAction(() => (T)Instance.Map(Input), "SOAP", "Map");
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void the_actual_output_is_the_expected_output()
        {
            ActualOutput.ShouldEqual(ExpectedOutput);
        }
    }

    [TestClass]
    public class when_a_string_is_mapped_with_a_valid_identity : SoapIdentityMapperContextSpecification<string>
    {
        public override void Given()
        {
            base.Given();
            Input = "rimuri";
            ExpectedOutput = "rimuri@microsoft.com";
        }
    }

    public abstract class IdentityMapperContextSpecification : WorkItemStoreComparisonContextSpecification
    {
        protected IOrderedQueryable<IWorkItem> SoapQueryable { get; private set; }
        protected IOrderedQueryable<IWorkItem> RestQueryable { get; private set; }



        /// <inheritdoc />
        public override void Given()
        {
            base.Given();

            var soapIms = ((Soap.IInternalTeamProjectCollection)Soap.TeamProjectCollection)
                            .GetService<IIdentityManagementService2>()
                            .AsProxy();
            var translator = new WiqlTranslator();
            var idMapper = new IdentityAliasValueConverter(soapIms, "72F988BF-86F1-41AF-91AB-2D7CD011DB47", "microsoft.com");
            var visitors = new ExpressionVisitor[]
                               {
                                   new PartialEvaluator(),
                                   new IdentityMappingVisitor(idMapper),
                                   new QueryRewriter()
                               };

            var soapBuilder = new WiqlQueryBuilder(translator, visitors);
            var soapQp = new TeamFoundationServerWorkItemQueryProvider(Soap, soapBuilder);
            SoapQueryable = new Query<IWorkItem>(soapQp, soapBuilder);

            var restBuilder = new WiqlQueryBuilder();
            var restQp = new TeamFoundationServerWorkItemQueryProvider(Rest, restBuilder);
            RestQueryable = new Query<IWorkItem>(restQp, restBuilder);
        }
    }

    [TestClass]
    public class Given_WorkItems_queried_by_LINQ_on_AssignedTo_with_IdentityVisitor_by_alias : IdentityMapperContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            RestResult.WorkItems = RestQueryable.Where(i => i.AssignedTo == "rimuri").ToArray().ToWorkItemCollection();
            SoapResult.WorkItems = SoapQueryable.Where(i => i.AssignedTo == "rimuri").ToArray().ToWorkItemCollection();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        public void SOAP_returned_results()
        {
            SoapResult.WorkItems.Count.ShouldBeGreaterThan(0);
        }
    }
}
