using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Client;

using TfsWorkItem = Microsoft.TeamFoundation.WorkItemTracking.Client;
using WorkItemLinkTypeCollection = Microsoft.Qwiq.Proxies.WorkItemLinkTypeCollection;

namespace Microsoft.Qwiq.Soap.Proxies
{
    /// <summary>
    ///     Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    ///     all the TFS libraries.
    /// </summary>
    internal class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly Lazy<WorkItemLinkTypeCollection> _linkTypes;

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTfsTeamProjectCollection> _tfs;

        private readonly Lazy<TfsWorkItem.WorkItemStore> _workItemStore;

        internal WorkItemStoreProxy(
            TfsTeamProjectCollection tfsNative,
            Func<TfsWorkItem.WorkItemStore, IQueryFactory> queryFactory)
            : this(
                () => ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                    new TfsTeamProjectCollectionProxy(tfsNative)),
                queryFactory)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<TfsWorkItem.WorkItemStore> wisFactory,
            Func<TfsWorkItem.WorkItemStore, IQueryFactory> queryFactory)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTfsTeamProjectCollection>(tpcFactory);
            _workItemStore = new Lazy<TfsWorkItem.WorkItemStore>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory.Invoke(_workItemStore?.Value));

            _linkTypes = new Lazy<WorkItemLinkTypeCollection>(
                () =>
                    {
                        return new WorkItemLinkTypeCollection(
                            _workItemStore.Value.WorkItemLinkTypes.Select(item => new WorkItemLinkTypeProxy(item)));
                    });
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<TfsWorkItem.WorkItemStore, IQueryFactory> queryFactory)
            : this(tpcFactory, () => tpcFactory?.Invoke()?.GetService<TfsWorkItem.WorkItemStore>(), queryFactory)
        {
        }

        public TfsCredentials AuthorizedCredentials => _tfs.Value.AuthorizedCredentials;

        public IFieldDefinitionCollection FieldDefinitions => ExceptionHandlingDynamicProxyFactory
            .Create<IFieldDefinitionCollection>(
                new FieldDefinitionCollectionProxy(_workItemStore.Value.FieldDefinitions));

        public IEnumerable<IProject> Projects
        {
            get
            {
                return _workItemStore.Value.Projects.Cast<TfsWorkItem.Project>()
                                     .Select(
                                         item => ExceptionHandlingDynamicProxyFactory.Create<IProject>(
                                             new ProjectProxy(item, this)));
            }
        }

        public ITfsTeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => _workItemStore.Value.TimeZone;

        public string UserDisplayName => _workItemStore.Value.UserDisplayName;

        public string UserIdentityName => _workItemStore.Value.UserIdentityName;

        public string UserSid => _workItemStore.Value.UserSid;

        public WorkItemLinkTypeCollection WorkItemLinkTypes => _linkTypes.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
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

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var ids2 = (int[])ids.ToArray().Clone();
            if (!ids2.Any()) return Enumerable.Empty<IWorkItem>();

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

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) if (_tfs.IsValueCreated) _tfs.Value?.Dispose();
        }
    }
}