using Elasticsearch.BulkAndSearch.Models;
using Nest;
using System;
using System.Collections.Generic;

namespace Elasticsearch.BulkAndSearch
{
    public class ElasticsearchCommand<T> : BaseElasticsearch<T>, IElasticsearchCommand<T> where T : class
    {
        public ElasticsearchCommand(
            ElasticsearchOptions options, 
            Func<string, T, string> generateIndexName)
            : base(ConnectionMode.Write, options, generateIndexName)
        { }

        public bool Bulk(IEnumerable<T> documents)
        {
            BulkDescriptor descriptor = new BulkDescriptor();
            foreach (var document in documents)
            {
                descriptor.Index<T>(i => i
                    .Index(this.GetIndexName(document))
                    .Document(document));
            }
            
            return this.ElasticClient.Bulk(descriptor).IsValid;
        }

        public bool Upsert(T document)
        {
            var index = this.GetIndexName(document);
            return this.ElasticClient.Index(document, i => i.Index(index)).IsValid;
        }
    }
}
