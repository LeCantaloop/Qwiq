using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Compilation;
using Microsoft.IE.IEPortal.Data.TeamFoundationServer.Linq;
using Microsoft.IE.Qwiq.Linq;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Relatives.WiqlExpressions;

namespace Microsoft.IE.Qwiq.Relatives.Linq
{
    public class RelativesAwareTeamFoundationServerWorkItemQueryProvider : TeamFoundationServerWorkItemQueryProvider
    {
        private readonly IFieldMapper _fieldMapper;

        public RelativesAwareTeamFoundationServerWorkItemQueryProvider(
            IWorkItemStore workItemStore,
            IWiqlQueryBuilder queryBuilder,
            IWorkItemMapper workItemMapper,
            IFieldMapper fieldMapper)
            : base(workItemStore, queryBuilder, workItemMapper)
        {
            _fieldMapper = fieldMapper;
        }

        /// <exception cref="NotSupportedException"><paramref name="workItemType"/> is not assignable to <see cref="IWorkItem"/>.</exception>
        protected override IEnumerable ExecuteRawQuery(Type workItemType, string queryString)
        {
            var workItems = WorkItemStore.Query(queryString);

            if (workItemType == typeof(IWorkItem))
            {
                // This is only for GetDescendents. If it is no longer using it we should get rid of it
                return (workItems);
            }
            if (typeof(IWorkItem).IsAssignableFrom(workItemType))
            {
                return WorkItemMapper.Create(workItemType, workItems);
            }


            throw new NotSupportedException($"Unknown work item type {workItemType.Name}.");
        }

        protected override object ExecuteImpl(Expression expression, Type itemType)
        {
            var query = (RelativesAwareTranslatedQuery)WiqlQueryBuilder.BuildQuery(expression);

            var results = query.WillEverHaveResults()
                ? ExecuteRawQuery(query.UnderlyingQueryType, query.ToQueryString())
                : Activator.CreateInstance(typeof(List<>).MakeGenericType(query.UnderlyingQueryType)) as IEnumerable;

            if (query.Relatives != null)
            {
                var asOf = query.AsOfDateTime ?? DateTime.Now;
                results = GetRelatives(results.Cast<IWorkItem>(), query.Relatives, itemType, asOf);
            }
            else if (query.Projections.Count > 0)
            {
                return Projector.Project(query.Projections, results.Cast<object>());
            }

            return results;
        }

        private IEnumerable GetRelatives(IEnumerable<IWorkItem> source, RelativesExpression relativesExpression, Type itemType, DateTime asOf)
        {
            var results = Activator
                            .CreateInstance(typeof(List<>)
                            .MakeGenericType(itemType))
                            as IList;

            var ids = source.Select(item => item.Id);
            if (!ids.Any())
            {
                return results;
            }

            var workItemLinks = WorkItemStore.QueryLinks(GetQueryStringForRelativesOf(ids, relativesExpression.AncestorType, relativesExpression.DescendentType, relativesExpression.Direction, asOf));

            var map = new Dictionary<int, List<int>>();
            workItemLinks.Where(link => link.SourceId == 0).ToList().ForEach(link => map.Add(link.TargetId, new List<int>()));
            foreach (var ancestorId in map.Keys)
            {
                map[ancestorId].AddRange(GetTransitiveLinks(ancestorId, workItemLinks).Select(child => child.TargetId));
            }

            ids = ids.Union(workItemLinks.Select(link => link.SourceId)).Union(workItemLinks.Select(link => link.TargetId)).Where(id => id != 0);
            var workItems = HydrateAndFilterItems(ids, relativesExpression.AncestorType, relativesExpression.DescendentType, asOf);

            foreach (var ancestorId in map.Keys)
            {
                var id = ancestorId;
                // Get the ancestor from the list of hydrated work items
                var ancestorWorkItem = workItems.Single(item => item.Id == ancestorId);

                // Filter the work items to just those IDs that are in the map for this ancestor.
                // NOTE: Please note that the strategy is to start with the hydrated work items and filter to
                // those in the map, and NOT to lookup every work item in the map. The latter strategy would
                // break because the map also contains the IDs of Task Groups that have been filtered out.
                // This way we start with work items that we already know exist.
                var descendentWorkItems = workItems.Where(wi => map[id].Contains(wi.Id));

                var ancestor = WorkItemMapper.Create(relativesExpression.AncestorType, new[] { ancestorWorkItem }).Cast<object>().Single();
                var descendents = WorkItemMapper.Create(relativesExpression.DescendentType, descendentWorkItems);

                var properTypedDescendents = GenerateListOfType(descendents, itemType);
                var result = Activator.CreateInstance(itemType, ancestor, properTypedDescendents);
                results.Add(result);
            }

            return results;
        }

        private static object GenerateListOfType(IEnumerable input, Type type)
        {
            var listType = type.GetGenericArguments()[1].GetGenericArguments()[0];
            var results = Activator.CreateInstance(typeof(List<>).MakeGenericType(listType)) as IList;
            foreach (var elem in input)
            {
                results.Add(elem);
            }
            return results;
        }

        private string GetQueryStringForRelativesOf(IEnumerable<int> ids, Type ancestorType, Type descendentType, RelativesQueryDirection direction, DateTime asOf)
        {
            var quotedDescendentIds = string.Join(", ", ids.Select(id => "'" + id + "'"));
            var queryString = Constants.WORK_HIERARCHY_QUERY
                .Replace("#{{Ids}}", quotedDescendentIds)
                .Replace("#{{AncestorType}}", _fieldMapper.GetWorkItemType(ancestorType))
                .Replace("#{{DescendentType}}", _fieldMapper.GetWorkItemType(descendentType))
                .Replace("#{{Direction}}", direction == RelativesQueryDirection.AncestorFromDescendent ? "Target" : "Source")
                .Replace("#{{AsOf}}", asOf.ToUniversalTime().ToString("u"));

            return queryString;
        }

        private IEnumerable<IWorkItem> HydrateAndFilterItems(IEnumerable<int> itemIds, Type tAncestor, Type tDescendent, DateTime asOf)
        {
            var ids = itemIds as int[] ?? itemIds.ToArray();
            if (!ids.Any())
            {
                return new IWorkItem[] { };
            }

            // Filter out work items that aren't the ancestor nor descendent type
            var ancestorName = _fieldMapper.GetWorkItemType(tAncestor);
            var descendentName = _fieldMapper.GetWorkItemType(tDescendent);

            var queryString = Constants.ID_QUERY
                .Replace("#{{Ids}}", string.Join(", ", ids))
                .Replace("#{{AsOf}}", asOf.ToUniversalTime().ToString("u"))
                .Replace("#{{Ancestor}}", ancestorName)
                .Replace("#{{Descendent}}", descendentName);
            var allWorkItems = ExecuteRawQuery(typeof(IWorkItem), queryString);

            return (IEnumerable<IWorkItem>)allWorkItems;
        }

        private IEnumerable<IWorkItemLinkInfo> GetTransitiveLinks(int parentId, IEnumerable<IWorkItemLinkInfo> links)
        {
            var children = new List<IWorkItemLinkInfo>();
            children.AddRange(links.Where(link => link.SourceId == parentId));

            return children.Union(children.SelectMany(child => GetTransitiveLinks(child.TargetId, links)));
        }

        private class Constants
        {
            static Constants()
            {

                var assemblies = GetAssemblies();
                var fields = GetFields(assemblies).SelectMany(s=>s).ToArray();
                var fieldList = string.Join(", ", fields.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(n => n));

                ID_QUERY = "SELECT " + fieldList + " FROM WorkItems WHERE [System.Id] IN (#{{Ids}}) AND [Work Item Type] IN ('#{{Ancestor}}', '#{{Descendent}}') ASOF '#{{AsOf}}'";
            }

            private static IEnumerable<IEnumerable<string>> GetFields(IEnumerable<Assembly> assemblies)
            {
                return assemblies
                        .SelectMany(GetLoadableTypes, (asm, type) => new { asm, type })
                        .Select(@t => new { @t, witAttribs = @t.type.GetCustomAttributes(typeof(WorkItemTypeAttribute), true) })
                        .Where(@t => (@t.witAttribs.Any()))
                        .Select(@t => @t.@t.type.GetProperties().SelectMany(@p => @p.GetCustomAttributes(typeof(FieldDefinitionAttribute), true)).Cast<FieldDefinitionAttribute>())
                        .Select(@a => @a.Select(attrib => "[" + attrib.GetFieldName() + "]"));
            }

            private static IEnumerable<Assembly> GetAssemblies()
            {
                Assembly[] assemblies;

                try
                {
                    // When hosting applications in IIS all assemblies are loaded into the AppDomain when the application first starts,
                    // but when the AppDomain is recycled by IIS the assemblies are then only loaded on demand. To avoid this issue use
                    // the GetReferencedAssemblies() method on System.Web.Compilation.BuildManager to get a list of the referenced assemblies
                    // instead. That will force the referenced assemblies to be loaded into the AppDomain immediately making them available
                    // for module scanning.

                    assemblies = BuildManager
                                    .GetReferencedAssemblies()
                                    .Cast<Assembly>()
                                    .ToArray();
                }
                catch (InvalidOperationException)
                {
                    // This can fail when the AppDomain is not hosted within IIS
                    // We encounter this during test
                    assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }
                return assemblies;
            }

            /// <summary>
            /// Safely returns the set of loadable types from an assembly.
            /// </summary>
            /// <param name="assembly">The <see cref="System.Reflection.Assembly"/> from which to load types.</param>
            /// <returns>
            /// The set of types from the <paramref name="assembly" />, or the subset
            /// of types that could be loaded if there was any error.
            /// </returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown if <paramref name="assembly" /> is <see langword="null" />.
            /// </exception>
            private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
            {
                // Algorithm from StackOverflow answer here:
                // http://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
                if (assembly == null) throw new ArgumentNullException(nameof(assembly));

                try
                {
                    return assembly.DefinedTypes.Select(t => t.AsType());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null);
                }
            }


            // This is used to tell TFS what fields to hydrate when doing a query.
            //
            // NOTE: To get best performance, put every field you may possibly use. Unlinke with SQL, the TFS proxy
            // will transparently roundtrip to the server to get fields that you access (but that aren't in the
            // original query). The lazy-loading is nice, but it means you'll take a performance hit without knowing
            // it. Specifying all the fields you'll use is eager-loading, and will improve performance.
            //
            // NOTE: The system doesn't seem to penalize you for having fields in the list that a work item type doesn't have.
            // For example, it's OK to have 'Func Spec URL' in the list when querying for bugs; as long as the field exists in the
            // schema.

            internal static readonly string ID_QUERY;

            internal const string WORK_HIERARCHY_QUERY = @"
SELECT [System.Id]
FROM WorkItemLinks
WHERE
(
    [Source].[System.WorkItemType] = '#{{AncestorType}}'
)
AND ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward')
AND
(
    [Target].[System.WorkItemType] = '#{{DescendentType}}'
)
AND [#{{Direction}}].[System.Id] In (#{{Ids}})
ASOF '#{{AsOf}}'
ORDER BY [System.Id]
mode(mustcontain)";
        }
    }
}