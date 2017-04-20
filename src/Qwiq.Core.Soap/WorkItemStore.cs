using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;

using TfsWorkItem = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    /// <summary>
    ///     Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    ///     all the TFS libraries.
    /// </summary>
    internal class WorkItemStore : IWorkItemStore
    {
        private readonly Lazy<IWorkItemLinkTypeCollection> _workItemLinkTypes;

        private readonly Lazy<IRegisteredLinkTypeCollection> _linkTypes;

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTeamProjectCollection> _tfs;

        private readonly Lazy<TfsWorkItem.WorkItemStore> _workItemStore;

        private readonly Lazy<IProjectCollection> _projects;

        internal WorkItemStore(
            Func<IInternalTeamProjectCollection> tpcFactory,
            Func<TfsWorkItem.WorkItemStore> wisFactory,
            Func<WorkItemStore, IQueryFactory> queryFactory,
            int pageSize = PageSizeLimits.MaxPageSize)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));

            if (pageSize < PageSizeLimits.DefaultPageSize || pageSize > PageSizeLimits.MaxPageSize)
                throw new PageSizeRangeException();

            _tfs = new Lazy<IInternalTeamProjectCollection>(tpcFactory);
            _workItemStore = new Lazy<TfsWorkItem.WorkItemStore>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory(this));

            _workItemLinkTypes = new Lazy<IWorkItemLinkTypeCollection>(
                () =>
                    {
                        return new WorkItemLinkTypeCollection(
                            _workItemStore.Value.WorkItemLinkTypes.Select(item => new WorkItemLinkType(item)));
                    });


            _linkTypes = new Lazy<IRegisteredLinkTypeCollection>(() => new RegisteredLinkTypeCollection(_workItemStore.Value.RegisteredLinkTypes.OfType<TfsWorkItem.RegisteredLinkType>().Select(item => new RegisteredLinkType(item.Name))));

            _projects = new Lazy<IProjectCollection>(() => new ProjectCollection(_workItemStore.Value.Projects));

            PageSize = pageSize;
        }

        internal WorkItemStore(
            Func<IInternalTeamProjectCollection> tpcFactory,
            Func<WorkItemStore, IQueryFactory> queryFactory,
            int pageSize = PageSizeLimits.MaxPageSize)
            : this(tpcFactory, () => tpcFactory?.Invoke()?.GetService<TfsWorkItem.WorkItemStore>(), queryFactory, pageSize)
        {
        }

        public int PageSize { get; }

        public ClientType ClientType => ClientType.Soap;

        public VssCredentials AuthorizedCredentials => _tfs.Value.AuthorizedCredentials;

        internal TfsWorkItem.WorkItemStore NativeWorkItemStore => _workItemStore.Value;

        public IFieldDefinitionCollection FieldDefinitions => ExceptionHandlingDynamicProxyFactory
            .Create<IFieldDefinitionCollection>(
                new FieldDefinitionCollection(_workItemStore.Value.FieldDefinitions));

        public IProjectCollection Projects => _projects.Value;

        public ITeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => _workItemStore.Value.TimeZone;

        public string UserDisplayName => _workItemStore.Value.UserDisplayName;

        public string UserAccountName => TeamProjectCollection.AuthorizedIdentity.GetUserAlias();

        public string UserSid => _workItemStore.Value.UserSid;

        public IWorkItemLinkTypeCollection WorkItemLinkTypes => _workItemLinkTypes.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IWorkItemCollection Query(string wiql, bool dayPrecision = true)
        {
            try
            {
                var query = _queryFactory.Value.Create(wiql, dayPrecision);
                return query.RunQuery();
            }
            catch (TfsWorkItem.ValidationException ex)
            {
                throw new ValidationException(ex);
            }
        }

        public IWorkItemCollection Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var ids2 = (int[])ids.ToArray().Clone();
            if (!ids2.Any()) return Enumerable.Empty<IWorkItem>().ToWorkItemCollection();

            try
            {
                var query = _queryFactory.Value.Create(ids2, asOf);
                return query.RunQuery();
            }
            catch (TfsWorkItem.ValidationException ex)
            {
                throw new ValidationException(ex);
            }
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }, asOf).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = true)
        {
            try
            {
                var query = _queryFactory.Value.Create(wiql, dayPrecision);
                return query.RunLinkQuery();
            }
            catch (TfsWorkItem.ValidationException ex)
            {
                throw new ValidationException(ex);
            }
        }

        public IRegisteredLinkTypeCollection RegisteredLinkTypes => _linkTypes.Value;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) if (_tfs.IsValueCreated) _tfs.Value?.Dispose();
        }
    }
}