using Elasticsearch.BulkAndSearch.Models;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticsearch.BulkAndSearch
{
    public class ElasticsearchCommand<TEntity> : BaseElasticsearch<TEntity>, IElasticsearchCommand<TEntity> where TEntity : class
    {
        public ElasticsearchCommand(
            ElasticsearchOptions options,
            Func<string, TEntity, string> generateIndexName)
            : base(ConnectionMode.Write, options, generateIndexName)
        {
        }

        public IBulkResponse Bulk(IEnumerable<TEntity> documents, string type = null)
        {
            var descriptor = this.BuildBulkDescriptor(documents, type);
            return this.ElasticClient.Bulk(descriptor);
        }

        public async Task<IBulkResponse> BulkAsync(IEnumerable<TEntity> documents, string type = null, CancellationToken cancellationToken = default)
        {
            var descriptor = this.BuildBulkDescriptor(documents, type);
            return await this.ElasticClient.BulkAsync(descriptor, cancellationToken);
        }

        public bool Upsert(TEntity document, string type = null, Refresh refresh = Refresh.False)
        {
            var index = this.GetIndexName(document);
            return this.ElasticClient.Index((object)document, i => DoUpsert(type, refresh, i, index)).IsValid;
        }

        public async Task<bool> UpsertAsync(TEntity document, string type = null, Refresh refresh = Refresh.False, CancellationToken cancellationToken = default)
        {
            var index = this.GetIndexName(document);
            var response = await this.ElasticClient.IndexAsync((object)document, i => DoUpsert(type, refresh, i, index), cancellationToken);
            return response.IsValid;
        }

        private BulkDescriptor BuildBulkDescriptor(IEnumerable<TEntity> documents, string type)
        {
            var descriptor = new BulkDescriptor();
            foreach (var document in documents)
            {
                descriptor
                    .Index<object>(bulkIndexDescriptor => bulkIndexDescriptor
                        .Index(this.GetIndexName(document))
                        .Type(type ?? this.Options.DefaultTypeName)
                        .Document(document));
            }

            return descriptor;
        }

        private IndexDescriptor<object> DoUpsert(string type, Refresh refresh, IndexDescriptor<object> i, string index)
        {
            return i.Index(index).Type(type ?? this.Options.DefaultTypeName).Refresh(refresh);
        }
    }
}