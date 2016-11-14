using System;
using System.Collections;
using Microsoft.Qwiq.Linq;

namespace Microsoft.Qwiq.Mapper
{
    public class MapperTeamFoundationServerWorkItemQueryProvider : TeamFoundationServerWorkItemQueryProvider
    {
        protected IWorkItemMapper WorkItemMapper;

        public MapperTeamFoundationServerWorkItemQueryProvider(IWorkItemStore workItemStore,
            IWiqlQueryBuilder queryBuilder) : this(workItemStore, queryBuilder, new SimpleWorkItemMapper())
        {
        }

        public MapperTeamFoundationServerWorkItemQueryProvider(IWorkItemStore workItemStore,
            IWiqlQueryBuilder queryBuilder, IWorkItemMapper workItemMapper) : base(workItemStore, queryBuilder)
        {
            WorkItemMapper = workItemMapper;
        }

        protected override IEnumerable ExecuteRawQuery(Type workItemType, string queryString)
        {
            var workItems = WorkItemStore.Query(queryString);
            return WorkItemMapper.Create(workItemType, workItems);
        }
    }
}
