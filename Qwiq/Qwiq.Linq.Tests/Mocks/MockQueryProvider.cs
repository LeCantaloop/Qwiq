using System;
using System.Collections;
using System.Linq;
using Microsoft.IE.Qwiq.Mapper;

namespace Microsoft.IE.Qwiq.Linq.Tests.Mocks
{
    internal class MockQueryProvider : TeamFoundationServerWorkItemQueryProvider
    {
        public MockQueryProvider(IWorkItemStore workItemStore, IWiqlQueryBuilder builder, IWorkItemMapper mapper)
            : base(workItemStore, builder, mapper)
        {
        }

        // TODO: MATTKOT: Replace this and below with mock WorkItemStore
        private IEnumerable GetMockWorkItems()
        {
            return new[]
            {
                new SimpleMockModel
                {
                    Id = 1,
                    IntField = 2
                },
                new SimpleMockModel
                {
                    Id = 2,
                    IntField = 4
                },
                new SimpleMockModel
                {
                    Id = 3,
                    IntField = 3
                },
                new SimpleMockModel
                {
                    Id = 4,
                    IntField = 4
                },
                new SimpleMockModel
                {
                    Id = 5,
                    IntField = 5
                }
            };
        }

        protected override IEnumerable ExecuteRawQuery(Type workItemType, string queryString)
        {
            if (queryString ==
                "SELECT [ID], [IntField] FROM WorkItems WHERE (([Work Item Type] = 'SimpleMockWorkItem'))")
            {
                return GetMockWorkItems();
            }
            if (queryString.StartsWith("SELECT [Id], [Title], [Priority], [Assigned To], [State], [Substate], [Closed By], [Iteration Path], [Created Date], [Closed Date], [Keywords], [Changed Date], [Revised Date], [Issue Type], [Triage], [Severity], [Resolved By], [Resolved Date], [Cost], [Resolved Reason], [Area Path], [Rank], [Created By], [Task Type], [Remaining Days], [Func Spec Status], [Func Spec URL], [Dev Design Status], [Quality Plan Status], [Quality Plan URL], [Combined Spec Status], [Combined Spec URL], [Dev Estimate], [Quality Estimate], [Subjective or Objective], [Measure Operator], [Measure Target], [OSG Unit Of Measure], [Measure Upper Limit], [Measure Lower Limit], [Measure Data Source], [Measure Result], [Measure Overall Status], [Measure Overall Status Comment], [Measure Status Date], [OSG Measure Type], [Tags], [How Found], [NumInstances], [Blocking], [Dev Cost], [Remaining Dev Days], [Tags], [History], [Custom String 04], [Impacted Area], [Hot Bug Type], [Measure Category], [Category] FROM WorkItems WHERE [Id] IN (1, 2, 3, 4, 5)"))
            {
                return WorkItemStore.Query("");
            }
            if (queryString ==
                "SELECT [ID], [IntField] FROM WorkItems WHERE (([ID] = -1) AND ([Work Item Type] = 'SimpleMockWorkItem'))")
            {
                return Enumerable.Empty<SimpleMockModel>();
            }
            throw new ArgumentException("Query not registered in the MockQueryProvider.");
        }
    }
}
