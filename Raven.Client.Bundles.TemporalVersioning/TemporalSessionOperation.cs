﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using Raven.Abstractions.Data;
using Raven.Client.Bundles.TemporalVersioning.Common;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace Raven.Client.Bundles.TemporalVersioning
{
    public class TemporalSessionOperation : ISyncTemporalSessionOperation
    {
        private readonly IDocumentSession _session;
        private readonly DateTimeOffset _effective;
        private readonly NameValueCollection _headers;

        internal TemporalSessionOperation(IDocumentSession session, DateTimeOffset effective)
        {
            _session = session;
            _effective = effective;
            _headers = ((DocumentSession) _session).DatabaseCommands.OperationsHeaders;
        }

        #region Load

        public T Load<T>(string id)
        {
            return TemporalLoad(() => _session.Load<T>(id));
        }

        public T[] Load<T>(params string[] ids)
        {
            return TemporalLoad(() => _session.Load<T>(ids));
        }

        public T[] Load<T>(IEnumerable<string> ids)
        {
            return TemporalLoad(() => _session.Load<T>(ids));
        }

        public T Load<T>(ValueType id)
        {
            return TemporalLoad(() => _session.Load<T>(id));
        }

        #endregion

        #region Query

        public IRavenQueryable<T> Query<T>()
        {
            return _session.Query<T>().Customize(IncludeTemporalEffectiveOnQuery());
        }

        public IRavenQueryable<T> Query<T>(string indexName)
        {
            return _session.Query<T>(indexName).Customize(IncludeTemporalEffectiveOnQuery());
        }

        public IRavenQueryable<T> Query<T, TIndexCreator>()
            where TIndexCreator : AbstractIndexCreationTask, new()
        {
            return _session.Query<T, TIndexCreator>().Customize(IncludeTemporalEffectiveOnQuery());
        }

        private Action<IDocumentQueryCustomization> IncludeTemporalEffectiveOnQuery()
        {
            // This gets stripped out later by the listener
            return x => x.Include("__TemporalEffective__=" + _effective.ToString("o"));
        }

        #endregion

        #region Include

        public ITemporalLoaderWithInclude<object> Include(string path)
        {
            return new TemporalMultiLoaderWithInclude<object>(this, _session.Include(path));
        }

        public ITemporalLoaderWithInclude<T> Include<T>(Expression<Func<T, object>> path)
        {
            return new TemporalMultiLoaderWithInclude<T>(this, _session.Include(path));
        }

        public ITemporalLoaderWithInclude<T> Include<T, TInclude>(Expression<Func<T, object>> path)
        {
            return new TemporalMultiLoaderWithInclude<T>(this, _session.Include<T, TInclude>(path));
        }

        #endregion

        #region Store

        public void Store(object entity, Etag etag)
        {
            _session.Store(entity, etag);
            PrepareNewRevision(entity);
        }

        public void Store(object entity, Etag etag, string id)
        {
            _session.Store(entity, etag, id);
            PrepareNewRevision(entity);
        }

        public void Store(dynamic entity)
        {
            _session.Store(entity);
            PrepareNewRevision(entity);
        }

        public void Store(dynamic entity, string id)
        {
            _session.Store(entity, id);
            PrepareNewRevision(entity);
        }

        private void PrepareNewRevision(object entity)
        {
            var temporal = _session.Advanced.GetTemporalMetadataFor(entity);
            temporal.Status = TemporalStatus.Revision;
            temporal.Effective = _effective;
        }

        #endregion

        internal T TemporalLoad<T>(Func<T> loadOperation)
        {
            // perform the load operation, passing the temporal effective date header just for this operation
            _headers.Add(TemporalMetadata.RavenTemporalEffective, _effective.ToString("o"));
            var result = loadOperation();
            _headers.Remove(TemporalMetadata.RavenTemporalEffective);

            return result;
        }
    }
}
