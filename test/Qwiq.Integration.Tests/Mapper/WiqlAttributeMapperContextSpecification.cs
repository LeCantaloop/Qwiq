using Qwiq.Linq;
using Qwiq.Linq.Visitors;
using Qwiq.Mapper.Attributes;
using Qwiq.Tests.Common;
using System.Linq;

namespace Qwiq.Mapper
{
    public abstract class WiqlAttributeMapperContextSpecification : TimedContextSpecification
    {
        private int[] _ids;
        public IQueryable<Bug> Bugs { get; set; }
        protected IWorkItemStore WorkItemStore { get; private set; }
        private Query<Bug> Query { get; set; }
        public override void Cleanup()
        {
            WorkItemStore?.Dispose();
            base.Cleanup();
        }

        public override void Given()
        {
            base.Given();

            WorkItemStore = TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "WIS Create");

            ConfigureOptions();

            var pr = new PropertyReflector();
            var pi = new PropertyInspector(pr);
            var attMapper = new AttributeMapperStrategy(pi);
            var mapper = new WorkItemMapper(attMapper);
            var translator = new WiqlTranslator();
            var pe = new PartialEvaluator();
            var qr = new QueryRewriter();
            var wqb = new WiqlQueryBuilder(translator, pe, qr);
            var qp = new MapperTeamFoundationServerWorkItemQueryProvider(
                WorkItemStore,
                wqb,
                mapper);

            Query = new Query<Bug>(qp, wqb);

            _ids = new[]
            {
                8663955
            };
        }

        public override void When()
        {
            Bugs = Query.Where(b => _ids.Contains(b.Id.Value));
        }

        protected abstract void ConfigureOptions();
        [WorkItemType("Bug")]
        public class Bug : IIdentifiable<int?>
        {
            [FieldDefinition(CoreFieldRefNames.Id, true)]
            public int? Id { get; set; }

            [FieldDefinition(CoreFieldRefNames.State)]
            public string State { get; set; }

            [FieldDefinition("InvalidField")]
            public string Invalid { get; set; }
        }
    }
}