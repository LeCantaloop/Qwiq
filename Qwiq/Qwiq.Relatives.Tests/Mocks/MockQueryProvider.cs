using System;
using System.Collections;
using System.Linq;
using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Relatives.Linq;

namespace Microsoft.Qwiq.Relatives.Tests.Mocks
{
    internal class MockQueryProvider : RelativesAwareTeamFoundationServerWorkItemQueryProvider
    {
        public MockQueryProvider(IWorkItemStore workItemStore, IWiqlQueryBuilder builder, IWorkItemMapper mapper,
            IFieldMapper fieldMapper) : base(workItemStore, builder, mapper, fieldMapper)
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
                    Priority = 2
                },
                new SimpleMockModel
                {
                    Id = 2,
                    Priority = 4
                },
                new SimpleMockModel
                {
                    Id = 3,
                    Priority = 3
                },
                new SimpleMockModel
                {
                    Id = 4,
                    Priority = 4
                },
                new SimpleMockModel
                {
                    Id = 5,
                    Priority = 5
                }
            };
        }

        protected override IEnumerable ExecuteRawQuery(Type workItemType, string queryString)
        {
            if (queryString ==
                "SELECT [ID], [Priority] FROM WorkItems WHERE (([Work Item Type] = 'SimpleMockWorkItem'))")
            {
                return GetMockWorkItems();
            }
            if (queryString.StartsWith("SELECT [Accessibility Scope Assessment], [Activated Date], [Area Path], [Assigned To], [Blocking], [Category], [Changed Date], [Closed By], [Closed Date], [Compliance], [Cost], [Created By], [Created Date], [Custom String 01], [Custom String 02], [Custom String 03], [Custom String 04], [Custom String 05], [Custom String 06], [Custom String 07], [Custom String 08], [Custom String 09], [Deliverable Owner], [Dev Cost], [Dev Design Status], [Dev Estimate], [DevOwner], [Engagement Status], [Found in Environment], [Func Spec Required], [Func Spec Status], [Func Spec URL], [Geopolitical Scope Assessment], [History], [How Found], [Id], [Issue Subtype Detail], [Issue Subtype], [Issue Type], [Iteration Path], [Keywords], [Language], [Measure Data Source], [Measure Lower Limit], [Measure Operator], [Measure Overall Status Comment], [Measure Overall Status], [Measure Result], [Measure Status Date], [Measure Target], [Measure Upper Limit], [NumInstances], [Original Estimate], [OSG Measure Type], [OSG Unit of Measure], [Partner Feature/Division], [Partner Name], [Partner Status], [Partner SubFeature/SubDiv], [Partner Type], [PMOwner], [Priority], [Privacy Scope Assessment], [Product Family], [Product], [Protocols Scope Assessment], [Quality Estimate], [Quality Plan Status], [Quality Plan URL], [QualityOwner], [Rank], [Reason], [Release Type], [Release], [Remaining Days], [Remaining Dev Days], [Repro Steps], [Resolved By], [Resolved Date], [Resolved Reason], [Revised Date], [Security Scope Assessment], [Severity], [ShowOnReport], [State], [Subjective or Objective], [Substate], [Substatus], [Tags], [Task Type], [Title], [Triage], [Visible To Partner] FROM WorkItems WHERE [System.TeamProject] = 'OS' AND [System.Id] IN (1, 2, 3, 4, 5)"))
            {
                return WorkItemStore.Query("");
            }
            if (queryString ==
                "SELECT [ID], [Priority] FROM WorkItems WHERE (([ID] = -1) AND ([Work Item Type] = 'SimpleMockWorkItem'))")
            {
                return Enumerable.Empty<SimpleMockModel>();
            }
            throw new ArgumentException("Query not registered in the MockQueryProvider.", "queryString", new InvalidOperationException("Unregistered query: " + queryString));
        }
    }
}

