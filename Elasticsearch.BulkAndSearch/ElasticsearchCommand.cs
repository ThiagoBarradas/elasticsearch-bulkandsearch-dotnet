using Elasticsearch.BulkAndSearch.Models;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public class ElasticsearchCommand<TEntity> : BaseElasticsearch<TEntity>, IElasticsearchCommand<TEntity> where TEntity : class
    {
        public ElasticsearchCommand(
            ElasticsearchOptions options, 
            Func<string, TEntity, string> generateIndexName)
            : base(ConnectionMode.Write, options, generateIndexName)
        { }

        public BulkResponse Bulk(IEnumerable<TEntity> documents)
        {
            BulkDescriptor descriptor = new BulkDescriptor();
            foreach (var document in documents)
            {
                descriptor.Index<object>(i => i
                    .Index(this.GetIndexName(document))
                    .Document(document));
            }

            return this.ElasticClient.Bulk(descriptor);
        }

        public bool Upsert(TEntity document, Refresh refresh = Refresh.False)
        {
            var index = this.GetIndexName(document);
            return this.ElasticClient.Index((object) document, i => 
                i.Index(index).Refresh(refresh)).IsValid;
        }
    }
}
