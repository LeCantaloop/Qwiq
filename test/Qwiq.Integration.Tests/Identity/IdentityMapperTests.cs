using System.Linq;
using System.Linq.Expressions;

using Qwiq.Client.Soap;
using Qwiq.Identity.Soap;
using Qwiq.Linq;
using Qwiq.Linq.Visitors;
using Qwiq.WorkItemStore;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Qwiq.Identity
{
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

            var soapIms = ((IInternalTeamProjectCollection)Soap.TeamProjectCollection).GetService<IIdentityManagementService2>().AsProxy();
            var translator = new WiqlTranslator();
            var idMapper = new IdentityAliasValueConverter(soapIms, IntegrationSettings.TenantId, IntegrationSettings.Domains);
            var visitors = new ExpressionVisitor[] { new PartialEvaluator(), new IdentityMappingVisitor(idMapper), new QueryRewriter() };

            var soapBuilder = new WiqlQueryBuilder(translator, visitors);
            var soapQp = new TeamFoundationServerWorkItemQueryProvider(Soap, soapBuilder);
            SoapQueryable = new Query<IWorkItem>(soapQp, soapBuilder);

            var restBuilder = new WiqlQueryBuilder();
            var restQp = new TeamFoundationServerWorkItemQueryProvider(Rest, restBuilder);
            RestQueryable = new Query<IWorkItem>(restQp, restBuilder);
        }
    }
}
